using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using DragEventArgs = System.Windows.DragEventArgs;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;
using ListViewItem = System.Windows.Controls.ListViewItem;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace ThemeForge.Themes.Dialogs
{
    public partial class EditableFavoritesPanel : UserControl
    {
        public event EventHandler<string> FolderNavigationRequested;
        public event EventHandler<string> FileSelectionRequested;
        
        private string _currentPath;
        private FileDialogSettings Settings => FileDialogSettings.Instance;

        public EditableFavoritesPanel()
        {
            InitializeComponent();
            LoadSystemFolders();
            LoadFavorites();
            LoadRecentFiles();
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                UpdateAddCurrentFolderState();
            }
        }

        private void LoadSystemFolders()
        {
            SystemFoldersListView.Items.Clear();
            
            var systemFolders = new[]
            {
                new { Name = "Desktop", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
                new { Name = "Documents", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                new { Name = "Downloads", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") },
                new { Name = "Pictures", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
                new { Name = "Music", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
                new { Name = "Videos", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
                new { Name = "This PC", Path = "ThisPC" }
            };

            foreach (var folder in systemFolders)
            {
                if (folder.Path == "ThisPC" || Directory.Exists(folder.Path))
                {
                    var item = new ListViewItem 
                    { 
                        Content = folder.Name, 
                        Tag = folder.Path 
                    };
                    SystemFoldersListView.Items.Add(item);
                }
            }
        }

        private void LoadFavorites()
        {
            FavoritesListView.Items.Clear();
            
            var favorites = Settings.FavoriteFolders.Where(f => Directory.Exists(f.Path)).ToList();
            
            foreach (var favorite in favorites)
            {
                var item = new ListViewItem 
                { 
                    Content = favorite.Name, 
                    Tag = favorite.Path,
                    ToolTip = favorite.Path
                };
                FavoritesListView.Items.Add(item);
            }
        }

        private void LoadRecentFiles()
        {
            RecentFilesListView.Items.Clear();
            
            var recentFiles = Settings.RecentFiles.Where(f => File.Exists(f)).Take(10).ToList();
            
            foreach (var filePath in recentFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var item = new
                {
                    Name = fileInfo.Name,
                    DirectoryName = fileInfo.DirectoryName,
                    FullPath = filePath
                };
                RecentFilesListView.Items.Add(item);
            }
        }

        private void UpdateAddCurrentFolderState()
        {
            // Enable/disable add current folder based on whether current path is valid
            AddFavoriteButton.IsEnabled = !string.IsNullOrEmpty(_currentPath) && Directory.Exists(_currentPath);
        }

        private void SystemFoldersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SystemFoldersListView.SelectedItem is ListViewItem item)
            {
                string path = item.Tag as string;
                if (path == "ThisPC")
                {
                    // Handle This PC specially
                    FolderNavigationRequested?.Invoke(this, path);
                }
                else if (Directory.Exists(path))
                {
                    FolderNavigationRequested?.Invoke(this, path);
                }
                
                // Clear selection
                SystemFoldersListView.SelectedItem = null;
            }
        }

        private void FavoritesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoritesListView.SelectedItem is ListViewItem item)
            {
                string path = item.Tag as string;
                if (Directory.Exists(path))
                {
                    FolderNavigationRequested?.Invoke(this, path);
                }
                
                // Clear selection
                FavoritesListView.SelectedItem = null;
            }
        }

        private void RecentFilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecentFilesListView.SelectedItem != null)
            {
                var item = RecentFilesListView.SelectedItem;
                var fullPath = item.GetType().GetProperty("FullPath")?.GetValue(item) as string;
                
                if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                {
                    FileSelectionRequested?.Invoke(this, fullPath);
                }
                
                // Clear selection
                RecentFilesListView.SelectedItem = null;
            }
        }

        private void AddFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddFavoriteContextMenu();
        }

        private void ShowAddFavoriteContextMenu()
        {
            var contextMenu = FindResource("AddFavoritesContextMenu") as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.PlacementTarget = AddFavoriteButton;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                contextMenu.IsOpen = true;
            }
        }

        private void AddCurrentFolder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPath) || !Directory.Exists(_currentPath))
            {
                CustomMessageBox.Show("No valid current folder to add.", "Add Favorite", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var folderName = Path.GetFileName(_currentPath);
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = _currentPath; // For root drives
            }

            var result = InputDialog.Show("Add Favorite Folder", "Enter a name for this favorite:", folderName);
            if (result.DialogResult == true && !string.IsNullOrEmpty(result.Result))
            {
                Settings.AddFavoriteFolder(result.Result, _currentPath);
                LoadFavorites();
            }
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            // Use a simple input dialog to get the folder path
            var result = InputDialog.Show("Add Favorite Folder", "Enter the full path to the folder:", "");
            if (result.DialogResult == true && !string.IsNullOrEmpty(result.Result))
            {
                string folderPath = result.Result.Trim();
                
                if (Directory.Exists(folderPath))
                {
                    var folderName = Path.GetFileName(folderPath);
                    if (string.IsNullOrEmpty(folderName))
                    {
                        folderName = folderPath; // For root drives
                    }

                    var nameResult = InputDialog.Show("Add Favorite Folder", "Enter a name for this favorite:", folderName);
                    if (nameResult.DialogResult == true && !string.IsNullOrEmpty(nameResult.Result))
                    {
                        Settings.AddFavoriteFolder(nameResult.Result, folderPath);
                        LoadFavorites();
                    }
                }
                else
                {
                    CustomMessageBox.Show("The specified folder does not exist.", "Invalid Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RemoveFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesListView.SelectedItem is ListViewItem item)
            {
                string path = item.Tag as string;
                string name = item.Content as string;
                
                if (CustomMessageBox.Show($"Remove '{name}' from favorites?", "Remove Favorite", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Settings.RemoveFavoriteFolder(path);
                    LoadFavorites();
                }
            }
        }

        private void RenameFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesListView.SelectedItem is ListViewItem item)
            {
                string path = item.Tag as string;
                string currentName = item.Content as string;
                
                var result = InputDialog.Show("Rename Favorite", "Enter new name:", currentName);
                if (result.DialogResult == true && !string.IsNullOrEmpty(result.Result))
                {
                    // Remove old and add new
                    Settings.RemoveFavoriteFolder(path);
                    Settings.AddFavoriteFolder(result.Result, path);
                    LoadFavorites();
                }
            }
        }

        private void FavoriteProperties_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesListView.SelectedItem is ListViewItem item)
            {
                string path = item.Tag as string;
                string name = item.Content as string;
                
                CustomMessageBox.Show($"Name: {name}\nPath: {path}", "Favorite Properties", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearRecentButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.Show("Clear all recent files?", "Clear Recent Files", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Settings.RecentFiles.Clear();
                Settings.Save();
                LoadRecentFiles();
            }
        }

        public void RefreshAll()
        {
            LoadSystemFolders();
            LoadFavorites();
            LoadRecentFiles();
        }

        #region Drag and Drop Support

        private void FavoritesListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FavoritesListView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FavoritesListView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (var filePath in files)
                    {
                        AddPathToFavorites(filePath);
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    var path = (string)e.Data.GetData(DataFormats.Text);
                    if (!string.IsNullOrEmpty(path))
                    {
                        AddPathToFavorites(path);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error adding to favorites: {ex.Message}", "Drag & Drop Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddPathToFavorites(string path)
        {
            try
            {
                string folderPath;
                string displayName;

                if (File.Exists(path))
                {
                    // If it's a file, add the folder containing the file
                    folderPath = Path.GetDirectoryName(path);
                    displayName = Path.GetFileName(folderPath);
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = folderPath; // For root drives
                    }
                }
                else if (Directory.Exists(path))
                {
                    // If it's a folder, add the folder itself
                    folderPath = path;
                    displayName = Path.GetFileName(folderPath);
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = folderPath; // For root drives
                    }
                }
                else
                {
                    CustomMessageBox.Show($"The path '{path}' does not exist.", "Invalid Path", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if already exists
                if (Settings.FavoriteFolders.Any(f => f.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase)))
                {
                    CustomMessageBox.Show($"'{displayName}' is already in favorites.", "Duplicate Favorite", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Ask for custom name
                var result = InputDialog.Show("Add to Favorites", 
                    $"Add '{displayName}' to favorites?\n\nYou can change the display name:", displayName);
                
                if (result.DialogResult == true && !string.IsNullOrEmpty(result.Result))
                {
                    Settings.AddFavoriteFolder(result.Result, folderPath);
                    LoadFavorites();
                    
                    CustomMessageBox.Show($"Added '{result.Result}' to favorites.", "Favorite Added", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error processing path '{path}': {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }

    // Simple input dialog for getting text input
    public class InputDialog : Window
    {
        public class InputDialogResult
        {
            public bool DialogResult { get; set; }
            public string Result { get; set; }
        }

        private TextBox _textBox;
        private Button _okButton;
        private Button _cancelButton;

        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            Title = title;
            Width = 400;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var promptLabel = new Label { Content = prompt, Margin = new Thickness(10) };
            Grid.SetRow(promptLabel, 0);
            grid.Children.Add(promptLabel);

            _textBox = new TextBox { Text = defaultValue, Margin = new Thickness(10, 0, 10, 10) };
            Grid.SetRow(_textBox, 1);
            grid.Children.Add(_textBox);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(10) };
            _okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(0, 0, 10, 0), IsDefault = true };
            _cancelButton = new Button { Content = "Cancel", Width = 75, IsCancel = true };
            
            _okButton.Click += (s, e) => { DialogResult = true; Close(); };
            _cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(_okButton);
            buttonPanel.Children.Add(_cancelButton);
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            Content = grid;
            
            Loaded += (s, e) => { _textBox.Focus(); _textBox.SelectAll(); };
        }

        public string Result => _textBox.Text;

        public static InputDialogResult Show(string title, string prompt, string defaultValue = "")
        {
            var dialog = new InputDialog(title, prompt, defaultValue);
            var result = dialog.ShowDialog();
            return new InputDialogResult { DialogResult = result == true, Result = dialog.Result };
        }
    }
}