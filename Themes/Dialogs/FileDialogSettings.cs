using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace ThemeForge.Themes.Dialogs
{
    public class FileDialogSettings
    {
        public List<string> NavigationHistory { get; set; } = new List<string>();
        public List<string> RecentFiles { get; set; } = new List<string>();
        public List<string> FrequentFolders { get; set; } = new List<string>();
        public List<FavoriteFolder> FavoriteFolders { get; set; } = new List<FavoriteFolder>();
        public string LastOpenDirectory { get; set; } = "";
        public string LastSaveDirectory { get; set; } = "";
        public int MaxHistoryItems { get; set; } = 50;
        public int MaxRecentFiles { get; set; } = 20;
        public int MaxFrequentFolders { get; set; } = 10;
        public FileDialogViewMode DefaultViewMode { get; set; } = FileDialogViewMode.Details;
        public bool RememberWindowSize { get; set; } = true;
        public double WindowWidth { get; set; } = 800;
        public double WindowHeight { get; set; } = 600;
        public bool ShowPreview { get; set; } = true;
        public bool ShowHiddenFiles { get; set; } = false;
        
        private static FileDialogSettings _instance;
        private static readonly object _lock = new object();
        private static readonly string _settingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ThemeForge", "FileDialogSettings.json");

        public static FileDialogSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = Load();
                        }
                    }
                }
                return _instance;
            }
        }

        private static FileDialogSettings Load()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    string json = File.ReadAllText(_settingsFile);
                    var settings = JsonConvert.DeserializeObject<FileDialogSettings>(json);
                    if (settings != null)
                    {
                        settings.InitializeDefaults();
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash - just use defaults
                System.Diagnostics.Debug.WriteLine($"Error loading file dialog settings: {ex.Message}");
            }

            var defaultSettings = new FileDialogSettings();
            defaultSettings.InitializeDefaults();
            return defaultSettings;
        }

        private void InitializeDefaults()
        {
            // Initialize default favorite folders if empty
            if (FavoriteFolders.Count == 0)
            {
                FavoriteFolders.AddRange(new[]
                {
                    new FavoriteFolder { Name = "Desktop", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), IsRemovable = false },
                    new FavoriteFolder { Name = "Documents", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), IsRemovable = false },
                    new FavoriteFolder { Name = "Downloads", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), IsRemovable = false },
                    new FavoriteFolder { Name = "Pictures", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), IsRemovable = false },
                    new FavoriteFolder { Name = "Music", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), IsRemovable = false },
                    new FavoriteFolder { Name = "Videos", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), IsRemovable = false }
                });
            }

            // Clean up invalid paths
            FavoriteFolders.RemoveAll(f => !Directory.Exists(f.Path));
            NavigationHistory.RemoveAll(h => !Directory.Exists(h));
            FrequentFolders.RemoveAll(f => !Directory.Exists(f));
            RecentFiles.RemoveAll(f => !File.Exists(f));
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile));
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(_settingsFile, json);
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error saving file dialog settings: {ex.Message}");
            }
        }

        public void AddToNavigationHistory(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            // Normalize path
            path = Path.GetFullPath(path);

            // Remove if already exists
            NavigationHistory.Remove(path);
            
            // Add to beginning
            NavigationHistory.Insert(0, path);
            
            // Limit size
            if (NavigationHistory.Count > MaxHistoryItems)
                NavigationHistory.RemoveRange(MaxHistoryItems, NavigationHistory.Count - MaxHistoryItems);

            Save();
        }

        public void AddToRecentFiles(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            // Normalize path
            filePath = Path.GetFullPath(filePath);

            // Remove if already exists
            RecentFiles.Remove(filePath);
            
            // Add to beginning
            RecentFiles.Insert(0, filePath);
            
            // Limit size
            if (RecentFiles.Count > MaxRecentFiles)
                RecentFiles.RemoveRange(MaxRecentFiles, RecentFiles.Count - MaxRecentFiles);

            Save();
        }

        public void AddToFrequentFolders(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return;

            // Remove if already exists
            FrequentFolders.Remove(folderPath);
            
            // Add to beginning
            FrequentFolders.Insert(0, folderPath);
            
            // Limit size
            if (FrequentFolders.Count > MaxFrequentFolders)
                FrequentFolders.RemoveRange(MaxFrequentFolders, FrequentFolders.Count - MaxFrequentFolders);

            Save();
        }

        public void AddFavoriteFolder(string name, string path)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            // Check if already exists
            if (FavoriteFolders.Any(f => f.Path.Equals(path, StringComparison.OrdinalIgnoreCase)))
                return;

            FavoriteFolders.Add(new FavoriteFolder { Name = name, Path = path, IsRemovable = true });
            Save();
        }

        public void RemoveFavoriteFolder(string path)
        {
            var favorite = FavoriteFolders.FirstOrDefault(f => f.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (favorite != null && favorite.IsRemovable)
            {
                FavoriteFolders.Remove(favorite);
                Save();
            }
        }

        public string GetBestInitialDirectory(string specifiedDirectory, string currentFilePath, bool isOpenDialog)
        {
            // 1. Use specified directory if valid
            if (!string.IsNullOrEmpty(specifiedDirectory) && Directory.Exists(specifiedDirectory))
                return specifiedDirectory;

            // 2. Use directory of current file if available
            if (!string.IsNullOrEmpty(currentFilePath) && File.Exists(currentFilePath))
            {
                var dir = Path.GetDirectoryName(currentFilePath);
                if (Directory.Exists(dir))
                    return dir;
            }

            // 3. Use last used directory for this dialog type
            string lastUsed = isOpenDialog ? LastOpenDirectory : LastSaveDirectory;
            if (!string.IsNullOrEmpty(lastUsed) && Directory.Exists(lastUsed))
                return lastUsed;

            // 4. Use most recent from navigation history
            if (NavigationHistory.Count > 0)
            {
                var recent = NavigationHistory.FirstOrDefault(h => Directory.Exists(h));
                if (!string.IsNullOrEmpty(recent))
                    return recent;
            }

            // 5. Fall back to Documents folder
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Gets a unified history that combines navigation, recent files directories, and frequent folders
        /// </summary>
        public List<string> GetUnifiedFolderHistory(int maxItems = 20)
        {
            var allFolders = new List<(string Path, DateTime LastUsed, int Priority)>();

            // Add navigation history (highest priority)
            for (int i = 0; i < NavigationHistory.Count; i++)
            {
                if (Directory.Exists(NavigationHistory[i]))
                {
                    allFolders.Add((NavigationHistory[i], DateTime.Now.AddDays(-i), 100 - i));
                }
            }

            // Add directories from recent files (medium priority)
            for (int i = 0; i < RecentFiles.Count; i++)
            {
                if (File.Exists(RecentFiles[i]))
                {
                    var dir = Path.GetDirectoryName(RecentFiles[i]);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && 
                        !allFolders.Any(f => f.Path.Equals(dir, StringComparison.OrdinalIgnoreCase)))
                    {
                        allFolders.Add((dir, DateTime.Now.AddDays(-i - 10), 50 - i));
                    }
                }
            }

            // Add frequent folders (lower priority)
            for (int i = 0; i < FrequentFolders.Count; i++)
            {
                if (Directory.Exists(FrequentFolders[i]) && 
                    !allFolders.Any(f => f.Path.Equals(FrequentFolders[i], StringComparison.OrdinalIgnoreCase)))
                {
                    allFolders.Add((FrequentFolders[i], DateTime.Now.AddDays(-i - 20), 25 - i));
                }
            }

            // Sort by priority (highest first), then by last used (most recent first)
            return allFolders
                .OrderByDescending(f => f.Priority)
                .ThenByDescending(f => f.LastUsed)
                .Take(maxItems)
                .Select(f => f.Path)
                .ToList();
        }

        /// <summary>
        /// Gets related folders based on the current folder
        /// </summary>
        public List<string> GetRelatedFolders(string currentFolder, int maxItems = 10)
        {
            if (string.IsNullOrEmpty(currentFolder) || !Directory.Exists(currentFolder))
                return new List<string>();

            var relatedFolders = new List<string>();

            try
            {
                // Add parent folder
                var parent = Directory.GetParent(currentFolder);
                if (parent != null && parent.Exists)
                {
                    relatedFolders.Add(parent.FullName);
                }

                // Add sibling folders (folders in the same parent directory)
                if (parent != null && parent.Exists)
                {
                    var siblings = parent.GetDirectories()
                        .Where(d => !d.FullName.Equals(currentFolder, StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .Select(d => d.FullName);
                    relatedFolders.AddRange(siblings);
                }

                // Add child folders if current folder has some
                var currentDir = new DirectoryInfo(currentFolder);
                var children = currentDir.GetDirectories()
                    .Take(3)
                    .Select(d => d.FullName);
                relatedFolders.AddRange(children);

                // Add folders that have been accessed recently from navigation history
                var recentlyVisited = NavigationHistory
                    .Where(h => h.StartsWith(currentFolder, StringComparison.OrdinalIgnoreCase) && 
                               h != currentFolder && Directory.Exists(h))
                    .Take(3);
                relatedFolders.AddRange(recentlyVisited);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting related folders: {ex.Message}");
            }

            return relatedFolders.Distinct().Take(maxItems).ToList();
        }
    }

    public class FavoriteFolder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsRemovable { get; set; } = true;
    }

    public enum FileDialogViewMode
    {
        Details,
        List,
        Tiles
    }
}