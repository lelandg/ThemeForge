using ThemeForge.Themes.Dialogs;
using System.IO;
using System.Linq;

namespace ThemeForge.Themes
{
    public static class DialogManager
    {
        private static FileDialogSettings Settings => FileDialogSettings.Instance;

        /// <summary>
        /// Shows a custom open file dialog with the specified filter.
        /// </summary>
        /// <param name="filter">The file filter string (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
        /// <param name="multiselect">Whether multiple files can be selected</param>
        /// <param name="initialDirectory">The initial directory to display (if null, smart directory selection is used)</param>
        /// <param name="title">The dialog title (if null, default title is used)</param>
        /// <param name="currentFilePath">Path to currently open file to help determine initial directory</param>
        /// <returns>The selected file path or null if canceled</returns>
        public static string ShowOpenFileDialog(string filter = "All files (*.*)|*.*", bool multiselect = false, 
            string initialDirectory = null, string title = null, string currentFilePath = null)
        {
            // Determine the initial directory using smart selection
            string startingDirectory = Settings.GetBestInitialDirectory(initialDirectory, currentFilePath, true);

            var dialog = new CustomOpenFileDialog(filter, multiselect);

            if (!string.IsNullOrEmpty(title))
                dialog.Title = title;

            if (!string.IsNullOrEmpty(startingDirectory))
                dialog.InitialDirectory = startingDirectory;

            if (dialog.ShowDialog() == true)
            {
                string result = dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(result))
                {
                    string directory = Path.GetDirectoryName(result);
                    Settings.LastOpenDirectory = directory;
                    Settings.AddToNavigationHistory(directory);
                    Settings.AddToRecentFiles(result);
                    Settings.AddToFrequentFolders(directory);
                    Settings.Save();
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Shows a custom open file dialog that allows multiple file selection.
        /// </summary>
        /// <param name="filter">The file filter string</param>
        /// <param name="initialDirectory">The initial directory to display</param>
        /// <param name="title">The dialog title</param>
        /// <param name="currentFilePath">Path to currently open file to help determine initial directory</param>
        /// <returns>An array of selected file paths or null if canceled</returns>
        public static string[] ShowOpenFileDialogMulti(string filter = "All files (*.*)|*.*", 
            string initialDirectory = null, string title = null, string currentFilePath = null)
        {
            // Determine the initial directory using smart selection
            string startingDirectory = Settings.GetBestInitialDirectory(initialDirectory, currentFilePath, true);

            var dialog = new CustomOpenFileDialog(filter, true);

            if (!string.IsNullOrEmpty(title))
                dialog.Title = title;

            if (!string.IsNullOrEmpty(startingDirectory))
                dialog.InitialDirectory = startingDirectory;

            if (dialog.ShowDialog() == true)
            {
                string[] results = dialog.SelectedFilePaths;
                if (results != null && results.Length > 0)
                {
                    string directory = Path.GetDirectoryName(results[0]);
                    Settings.LastOpenDirectory = directory;
                    Settings.AddToNavigationHistory(directory);
                    Settings.AddToFrequentFolders(directory);
                    
                    // Add all selected files to recent files
                    foreach (string file in results)
                    {
                        Settings.AddToRecentFiles(file);
                    }
                    
                    Settings.Save();
                }
                return results;
            }
            return null;
        }

        /// <summary>
        /// Shows a custom save file dialog.
        /// </summary>
        /// <returns>The selected file path or null if canceled</returns>
        public static string ShowSaveFileDialog()
        {
            return ShowSaveFileDialog("All files (*.*)|*.*", null, null, null, null);
        }

        /// <summary>
        /// Shows a custom save file dialog with the specified filter.
        /// </summary>
        /// <param name="filter">The file filter string</param>
        /// <param name="initialDirectory">The initial directory to display</param>
        /// <param name="title">The dialog title</param>
        /// <param name="defaultFileName">Default filename to populate</param>
        /// <param name="currentFilePath">Path to currently open file to help determine initial directory</param>
        /// <returns>The selected file path or null if canceled</returns>
        public static string ShowSaveFileDialog(string filter = "All files (*.*)|*.*", 
            string initialDirectory = null, string title = null, string defaultFileName = null, string currentFilePath = null)
        {
            // Determine the initial directory using smart selection
            string startingDirectory = Settings.GetBestInitialDirectory(initialDirectory, currentFilePath, false);

            var dialog = new CustomSaveFileDialog(filter);

            if (!string.IsNullOrEmpty(title))
                dialog.Title = title;

            if (!string.IsNullOrEmpty(startingDirectory))
                dialog.InitialDirectory = startingDirectory;

            if (!string.IsNullOrEmpty(defaultFileName))
                dialog.DefaultFileName = defaultFileName;

            if (dialog.ShowDialog() == true)
            {
                string result = dialog.FilePath;
                if (!string.IsNullOrEmpty(result))
                {
                    string directory = Path.GetDirectoryName(result);
                    Settings.LastSaveDirectory = directory;
                    Settings.AddToNavigationHistory(directory);
                    Settings.AddToFrequentFolders(directory);
                    Settings.Save();
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Gets the recent files list from settings.
        /// </summary>
        /// <returns>List of recent file paths</returns>
        public static List<string> GetRecentFiles()
        {
            return Settings.RecentFiles.Where(f => File.Exists(f)).ToList();
        }

        /// <summary>
        /// Gets the frequent folders list from settings.
        /// </summary>
        /// <returns>List of frequent folder paths</returns>
        public static List<string> GetFrequentFolders()
        {
            return Settings.FrequentFolders.Where(f => Directory.Exists(f)).ToList();
        }

        /// <summary>
        /// Gets the navigation history from settings.
        /// </summary>
        /// <returns>List of navigation history paths</returns>
        public static List<string> GetNavigationHistory()
        {
            return Settings.NavigationHistory.Where(h => Directory.Exists(h)).ToList();
        }

        /// <summary>
        /// Gets the favorite folders from settings.
        /// </summary>
        /// <returns>List of favorite folders</returns>
        public static List<FavoriteFolder> GetFavoriteFolders()
        {
            return Settings.FavoriteFolders.Where(f => Directory.Exists(f.Path)).ToList();
        }

        /// <summary>
        /// Adds a favorite folder.
        /// </summary>
        /// <param name="name">Display name for the folder</param>
        /// <param name="path">Path to the folder</param>
        public static void AddFavoriteFolder(string name, string path)
        {
            Settings.AddFavoriteFolder(name, path);
        }

        /// <summary>
        /// Removes a favorite folder.
        /// </summary>
        /// <param name="path">Path to the folder to remove</param>
        public static void RemoveFavoriteFolder(string path)
        {
            Settings.RemoveFavoriteFolder(path);
        }
    }
}