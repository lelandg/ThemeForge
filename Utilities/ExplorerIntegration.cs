using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ThemeForge.Utilities
{
    /// <summary>
    /// Provides integration with Windows Explorer features like address history and recent folders
    /// </summary>
    public static class ExplorerIntegration
    {
        private const int CSIDL_RECENT = 0x0008;
        private const int MAX_PATH = 260;

        [DllImport("shell32.dll")]
        private static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, System.Text.StringBuilder lpszPath, int nFolder, bool fCreate);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern bool PathFileExists(string path);

        /// <summary>
        /// Gets Windows Explorer's recent folder history from registry
        /// </summary>
        public static List<string> GetExplorerRecentFolders()
        {
            var recentFolders = new List<string>();

            try
            {
                // Try to read from Windows Explorer's typed paths
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            var path = key.GetValue(valueName) as string;
                            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                            {
                                recentFolders.Add(path);
                            }
                        }
                    }
                }

                // Also try to read from Windows Explorer's address bar history
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU"))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            if (valueName != "MRUList")
                            {
                                var data = key.GetValue(valueName) as byte[];
                                if (data != null)
                                {
                                    var path = ExtractPathFromPidlMRU(data);
                                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path) && !recentFolders.Contains(path))
                                    {
                                        recentFolders.Add(path);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error reading Explorer history: {ex.Message}");
            }

            return recentFolders.Take(20).ToList();
        }

        /// <summary>
        /// Gets recently accessed files from Windows Recent folder
        /// </summary>
        public static List<string> GetRecentFiles()
        {
            var recentFiles = new List<string>();

            try
            {
                var recentPath = new System.Text.StringBuilder(MAX_PATH);
                if (SHGetSpecialFolderPath(IntPtr.Zero, recentPath, CSIDL_RECENT, false))
                {
                    var recentDir = new DirectoryInfo(recentPath.ToString());
                    if (recentDir.Exists)
                    {
                        var shortcuts = recentDir.GetFiles("*.lnk")
                            .OrderByDescending(f => f.LastWriteTime)
                            .Take(20);

                        foreach (var shortcut in shortcuts)
                        {
                            var targetPath = ResolveShortcut(shortcut.FullName);
                            if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                            {
                                recentFiles.Add(targetPath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading recent files: {ex.Message}");
            }

            return recentFiles;
        }

        /// <summary>
        /// Adds a path to Windows Explorer's typed paths history
        /// </summary>
        public static void AddToExplorerHistory(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"))
                {
                    if (key != null)
                    {
                        // Find next available slot (url1, url2, etc.)
                        int nextIndex = 1;
                        while (key.GetValue($"url{nextIndex}") != null && nextIndex <= 25)
                        {
                            var existingPath = key.GetValue($"url{nextIndex}") as string;
                            if (existingPath == path)
                                return; // Already exists
                            nextIndex++;
                        }

                        if (nextIndex <= 25)
                        {
                            key.SetValue($"url{nextIndex}", path);
                        }
                        else
                        {
                            // Shift existing entries and add new one at url1
                            var paths = new List<string>();
                            for (int i = 1; i <= 25; i++)
                            {
                                var existingPath = key.GetValue($"url{i}") as string;
                                if (!string.IsNullOrEmpty(existingPath) && existingPath != path)
                                {
                                    paths.Add(existingPath);
                                }
                            }

                            // Clear all entries
                            for (int i = 1; i <= 25; i++)
                            {
                                try { key.DeleteValue($"url{i}"); } catch { }
                            }

                            // Add new path first
                            key.SetValue("url1", path);

                            // Add back existing paths (up to 24)
                            for (int i = 0; i < Math.Min(paths.Count, 24); i++)
                            {
                                key.SetValue($"url{i + 2}", paths[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding to Explorer history: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets folder suggestions based on typed text
        /// </summary>
        public static List<string> GetFolderSuggestions(string partialPath, int maxSuggestions = 10)
        {
            var suggestions = new List<string>();

            if (string.IsNullOrEmpty(partialPath))
                return suggestions;

            try
            {
                // If it looks like a path, try to complete it
                if (partialPath.Contains('\\') || partialPath.Contains('/'))
                {
                    var directoryPart = Path.GetDirectoryName(partialPath);
                    var filenamePart = Path.GetFileName(partialPath);

                    if (!string.IsNullOrEmpty(directoryPart) && Directory.Exists(directoryPart))
                    {
                        var dir = new DirectoryInfo(directoryPart);
                        var matches = dir.GetDirectories($"{filenamePart}*")
                            .Take(maxSuggestions)
                            .Select(d => d.FullName);
                        suggestions.AddRange(matches);
                    }
                }
                else
                {
                    // Try drive roots
                    var drives = DriveInfo.GetDrives()
                        .Where(d => d.Name.StartsWith(partialPath, StringComparison.OrdinalIgnoreCase))
                        .Select(d => d.Name)
                        .Take(maxSuggestions);
                    suggestions.AddRange(drives);

                    // Try common folders from Explorer history
                    var explorerFolders = GetExplorerRecentFolders()
                        .Where(f => Path.GetFileName(f).StartsWith(partialPath, StringComparison.OrdinalIgnoreCase))
                        .Take(maxSuggestions - suggestions.Count);
                    suggestions.AddRange(explorerFolders);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting folder suggestions: {ex.Message}");
            }

            return suggestions.Distinct().ToList();
        }

        /// <summary>
        /// Gets file suggestions based on typed text in a directory
        /// </summary>
        public static List<string> GetFileSuggestions(string directory, string partialFilename, string[] extensions = null, int maxSuggestions = 10)
        {
            var suggestions = new List<string>();

            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(partialFilename) || !Directory.Exists(directory))
                return suggestions;

            try
            {
                var dir = new DirectoryInfo(directory);
                var files = dir.GetFiles($"{partialFilename}*");

                if (extensions != null && extensions.Length > 0)
                {
                    files = files.Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                }

                suggestions.AddRange(files.Take(maxSuggestions).Select(f => f.Name));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting file suggestions: {ex.Message}");
            }

            return suggestions;
        }

        private static string ExtractPathFromPidlMRU(byte[] data)
        {
            try
            {
                // This is a simplified extraction - PIDL format is complex
                // In a real implementation, you'd need to properly parse the PIDL structure
                var text = System.Text.Encoding.Unicode.GetString(data);
                
                // Look for drive letters or common path patterns
                var lines = text.Split('\0');
                foreach (var line in lines)
                {
                    if (line.Length > 2 && line[1] == ':' && Directory.Exists(line))
                    {
                        return line;
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return null;
        }

        private static string ResolveShortcut(string shortcutPath)
        {
            try
            {
                var shell = new object(); // Would use IWshRuntimeLibrary.WshShell in full implementation
                // For now, return null as we'd need additional COM interop
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}