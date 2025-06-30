using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Orientation = System.Windows.Controls.Orientation;
using SelectionMode = System.Windows.Controls.SelectionMode;

namespace ThemeForge.Themes.Dialogs
{
    public partial class FileSystemItem
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public bool IsDirectory { get; set; }
        public ImageSource Icon { get; set; }
    }

    public partial class CustomOpenFileDialog : Window
    {
        public string SelectedFilePath { get; private set; }
        public string[] SelectedFilePaths { get; private set; }
        public bool Multiselect { get; set; }
        public string InitialDirectory { get; set; }

        public new string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        public string Filter
        {
            get => _filter;
            set { _filter = value; ParseAndPopulateFilters(value); }
        }

        private string _filter;
        private string _currentPath;
        private List<string> _navigationHistory = new List<string>();
        private int _currentHistoryIndex = -1;
        private List<KeyValuePair<string, string>> _filterList = new List<KeyValuePair<string, string>>();
        private string _selectedFilterExtension = "*.*";

        public CustomOpenFileDialog(string filter = "All files (*.*)|*.*", bool multiselect = false)
        {
            InitializeComponent();
            Style = (Style)Application.Current.Resources["CustomWindowStyle"];
            Multiselect = multiselect;
            FileListView.SelectionMode = Multiselect ? SelectionMode.Extended : SelectionMode.Single;

            // Initial directory will be set in Loaded event
            _currentPath = Directory.GetCurrentDirectory();

            // Set filter - will call ParseAndPopulateFilters through the property setter
            Filter = filter;

            // Initial UI setup
            DetailsViewButton.IsChecked = true;

            // Subscribe to the Loaded event
            Loaded += CustomOpenFileDialog_Loaded;
        }

        private void CustomOpenFileDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the initial directory if specified
            string startDirectory = !string.IsNullOrEmpty(InitialDirectory) && Directory.Exists(InitialDirectory) 
                ? InitialDirectory 
                : Directory.GetCurrentDirectory();

            _currentPath = startDirectory;
            _navigationHistory.Add(_currentPath);
            _currentHistoryIndex = 0;

            PathComboBox.Text = _currentPath;
            UpdateNavigationButtons();
            PopulateFiles(_currentPath);
        }

        private void ParseAndPopulateFilters(string filter)
        {
            _filterList.Clear();

            if (string.IsNullOrEmpty(filter))
            {
                _filterList.Add(new KeyValuePair<string, string>("All files (*.*)", "*.*"));
            }
            else
            {
                string[] filters = filter.Split('|');
                for (int i = 0; i < filters.Length; i += 2)
                {
                    if (i + 1 < filters.Length)
                    {
                        string description = filters[i];
                        string pattern = filters[i + 1];
                        _filterList.Add(new KeyValuePair<string, string>(description, pattern));
                    }
                }

                if (_filterList.Count == 0)
                {
                    _filterList.Add(new KeyValuePair<string, string>("All files (*.*)", "*.*"));
                }
            }

            FileTypeComboBox.ItemsSource = _filterList;
            FileTypeComboBox.DisplayMemberPath = "Key";
            FileTypeComboBox.SelectedIndex = 0;

            // Set the initial selected filter extension
            if (FileTypeComboBox.SelectedItem is KeyValuePair<string, string> selectedFilter)
            {
                _selectedFilterExtension = selectedFilter.Value;

                // Refresh file list with new filter if we're already loaded
                if (IsLoaded && !string.IsNullOrEmpty(_currentPath))
                {
                    PopulateFiles(_currentPath);
                }
            }
            else
            {
                _selectedFilterExtension = "*.*";
            }
        }

        private void PopulateFiles(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                var items = new List<FileSystemItem>();

                // Add directories
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    try
                    {
                        items.Add(new FileSystemItem
                        {
                            Name = dir.Name,
                            FullName = dir.FullName,
                            LastWriteTime = dir.LastWriteTime,
                            Type = "File folder",
                            Size = 0,
                            IsDirectory = true
                        });
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip folders we don't have access to
                    }
                }

                // Add files
                var files = directoryInfo.GetFiles();
                var filteredFiles = files.Where(file => MatchesFilter(file.Name)).ToList();

                foreach (var file in filteredFiles)
                {
                    string fileType = GetFileTypeDescription(file.Extension);
                    items.Add(new FileSystemItem
                    {
                        Name = file.Name,
                        FullName = file.FullName,
                        LastWriteTime = file.LastWriteTime,
                        Type = fileType,
                        Size = file.Length,
                        IsDirectory = false
                    });
                }

                // Update UI
                FileListView.ItemsSource = items;
                _currentPath = path;
                PathComboBox.Text = _currentPath;
            }
            catch (UnauthorizedAccessException)
            {
                CustomMessageBox.Show("You don't have permission to access this folder.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error accessing path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetFileTypeDescription(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return "File";

            switch (extension.ToLower())
            {
                case ".txt": return "Text Document";
                case ".doc": case ".docx": return "Microsoft Word Document";
                case ".xls": case ".xlsx": return "Microsoft Excel Spreadsheet";
                case ".ppt": case ".pptx": return "Microsoft PowerPoint Presentation";
                case ".pdf": return "PDF Document";
                case ".jpg": case ".jpeg": return "JPEG Image";
                case ".png": return "PNG Image";
                case ".gif": return "GIF Image";
                case ".bmp": return "Bitmap Image";
                case ".mp3": return "MP3 Audio";
                case ".mp4": return "MP4 Video";
                case ".zip": return "ZIP Archive";
                case ".rar": return "RAR Archive";
                case ".exe": return "Application";
                case ".dll": return "Application Extension";
                case ".obj": return "OBJ 3D Model";
                case ".stl": return "STL 3D Model";
                case ".ply": return "PLY 3D Model";
                case ".json": return "JSON File";
                case ".xml": return "XML File";
                case ".html": case ".htm": return "HTML Document";
                case ".css": return "CSS File";
                case ".js": return "JavaScript File";
                case ".cs": return "C# Source Code";
                case ".vb": return "Visual Basic Source Code";
                case ".py": return "Python Source Code";
                case ".java": return "Java Source Code";
                case ".cpp": case ".c": case ".h": return "C/C++ Source Code";
                default: return $"{extension.TrimStart('.')} File";
            }
        }

        private bool MatchesFilter(string fileName)
        {
            if (_selectedFilterExtension == "*.*")
                return true;

            // Handle multiple extensions in the filter (e.g., "*.jpg;*.png")
            var patterns = _selectedFilterExtension.Split(';');
            foreach (var pattern in patterns)
            {
                string trimmedPattern = pattern.Trim();

                // For patterns like *.jpg
                if (trimmedPattern.StartsWith("*."))
                {
                    string extension = trimmedPattern.Substring(1); // Get the .jpg part
                    if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                // For other patterns (like just .jpg or specific filenames)
                else
                {
                    if (trimmedPattern.StartsWith(".") && fileName.EndsWith(trimmedPattern, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else if (trimmedPattern == fileName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddToHistory(string path)
        {
            // Remove any forward history when navigating to a new path
            if (_currentHistoryIndex < _navigationHistory.Count - 1)
            {
                _navigationHistory.RemoveRange(_currentHistoryIndex + 1, _navigationHistory.Count - _currentHistoryIndex - 1);
            }

            _navigationHistory.Add(path);
            _currentHistoryIndex = _navigationHistory.Count - 1;
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            BackButton.IsEnabled = _currentHistoryIndex > 0;
            ForwardButton.IsEnabled = _currentHistoryIndex < _navigationHistory.Count - 1;
        }

        private void NavigateToPath(string path, bool addToHistory = true)
        {
            if (Directory.Exists(path))
            {
                if (addToHistory && path != _currentPath)
                {
                    AddToHistory(path);
                }
                PopulateFiles(path);
            }
            else
            {
                CustomMessageBox.Show("Directory not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PathComboBox.Text = _currentPath;
            }
        }

        private void NavigateToSpecialFolder(Environment.SpecialFolder folder)
        {
            try
            {
                string path = Environment.GetFolderPath(folder);
                if (Directory.Exists(path))
                {
                    NavigateToPath(path);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error accessing special folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSelectedFileInTextBox()
        {
            if (FileListView.SelectedItem is FileSystemItem item && !item.IsDirectory)
            {
                FileNameTextBox.Text = item.Name;
            }
        }

        #region Event Handlers

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHistoryIndex > 0)
            {
                _currentHistoryIndex--;
                NavigateToPath(_navigationHistory[_currentHistoryIndex], false);
                UpdateNavigationButtons();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHistoryIndex < _navigationHistory.Count - 1)
            {
                _currentHistoryIndex++;
                NavigateToPath(_navigationHistory[_currentHistoryIndex], false);
                UpdateNavigationButtons();
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = Directory.GetParent(_currentPath);
            if (parent != null)
            {
                NavigateToPath(parent.FullName);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateFiles(_currentPath);
        }

        private void NavigationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationListView.SelectedItem is ListViewItem item)
            {
                string tag = item.Tag as string;
                switch (tag)
                {
                    case "Desktop":
                        NavigateToSpecialFolder(Environment.SpecialFolder.Desktop);
                        break;
                    case "Documents":
                        NavigateToSpecialFolder(Environment.SpecialFolder.MyDocuments);
                        break;
                    case "Downloads":
                        // Navigate to Downloads folder (not a SpecialFolder enum)
                        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                        if (Directory.Exists(downloadsPath))
                        {
                            NavigateToPath(downloadsPath);
                        }
                        break;
                    case "Pictures":
                        NavigateToSpecialFolder(Environment.SpecialFolder.MyPictures);
                        break;
                    case "Music":
                        NavigateToSpecialFolder(Environment.SpecialFolder.MyMusic);
                        break;
                    case "Videos":
                        NavigateToSpecialFolder(Environment.SpecialFolder.MyVideos);
                        break;
                    case "ThisPC":
                        // Show drives
                        PopulateDrives();
                        break;
                    case "Network":
                        // Network is more complex, so for now just display a message
                        CustomMessageBox.Show("Network browsing not implemented in this dialog.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
                // Clear selection after navigating
                NavigationListView.SelectedItem = null;
            }
        }

        private void PopulateDrives()
        {
            try
            {
                var items = new List<FileSystemItem>();
                foreach (var drive in DriveInfo.GetDrives())
                {
                    try
                    {
                        string driveType = drive.DriveType.ToString();
                        string name = !string.IsNullOrEmpty(drive.VolumeLabel) ? $"{drive.Name} ({drive.VolumeLabel})" : drive.Name;

                        items.Add(new FileSystemItem
                        {
                            Name = name,
                            FullName = drive.Name,
                            Type = $"{driveType} Drive",
                            IsDirectory = true
                        });
                    }
                    catch
                    {
                        // Skip drives with issues
                    }
                }

                FileListView.ItemsSource = items;
                _currentPath = "This PC";
                PathComboBox.Text = _currentPath;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error listing drives: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileListView.SelectedItem is FileSystemItem item)
            {
                if (item.IsDirectory)
                {
                    NavigateToPath(item.FullName);
                }
                else
                {
                    if (Multiselect)
                    {
                        SelectedFilePaths = FileListView.SelectedItems.Cast<FileSystemItem>().Select(f => f.FullName).ToArray();
                    }
                    else
                    {
                        SelectedFilePath = item.FullName;
                    }
                    DialogResult = true;
                }
            }
        }

        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedFileInTextBox();

            // Update preview if a file is selected
            if (FileListView.SelectedItem is FileSystemItem item && !item.IsDirectory)
            {
                PreviewControl.ShowPreview(item.FullName);
            }
            else
            {
                // Clear preview when directory or nothing is selected
                PreviewControl.ShowPreview(null);
            }
        }

        private void PathComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToPath(PathComboBox.Text);
            }
        }

        private void FileNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string fileName = FileNameTextBox.Text;

                // Check if it's a full path
                if (Path.IsPathRooted(fileName))
                {
                    // If it's a directory, navigate to it
                    if (Directory.Exists(fileName))
                    {
                        NavigateToPath(fileName);
                        return;
                    }

                    // If it's a file that exists
                    if (File.Exists(fileName))
                    {
                        SelectedFilePath = fileName;
                        DialogResult = true;
                        return;
                    }
                }
                else
                {
                    // Combine with current path to see if it exists
                    string fullPath = Path.Combine(_currentPath, fileName);

                    if (Directory.Exists(fullPath))
                    {
                        NavigateToPath(fullPath);
                        return;
                    }

                    if (File.Exists(fullPath))
                    {
                        SelectedFilePath = fullPath;
                        DialogResult = true;
                        return;
                    }
                }

                // If we got here and we're in a file selection mode, try to use the filename
                if (!string.IsNullOrEmpty(fileName) && FileListView.ItemsSource is IEnumerable<FileSystemItem> items)
                {
                    // If there are any matching files with this name in the current directory
                    var matchingFiles = items.Where(i => !i.IsDirectory && i.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (matchingFiles.Any())
                    {
                        if (Multiselect)
                        {
                            SelectedFilePaths = matchingFiles.Select(f => f.FullName).ToArray();
                        }
                        else
                        {
                            SelectedFilePath = matchingFiles.First().FullName;
                        }
                        DialogResult = true;
                        return;
                    }
                }

                // If it's a new file, use the filename for save dialogs
                // For open dialogs, we would typically show an error, but for now, let's just open it
                OpenButton_Click(sender, e);
            }
        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileTypeComboBox.SelectedItem is KeyValuePair<string, string> selectedFilter)
            {
                _selectedFilterExtension = selectedFilter.Value;
                PopulateFiles(_currentPath); // Refresh with new filter
            }
        }

        private void DetailsViewButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewButton.IsChecked = false;
            TilesViewButton.IsChecked = false;
            DetailsViewButton.IsChecked = true;

            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn { Header = "Name", Width = 300, DisplayMemberBinding = new System.Windows.Data.Binding("Name") });
            gridView.Columns.Add(new GridViewColumn { Header = "Date modified", Width = 150, DisplayMemberBinding = new System.Windows.Data.Binding("LastWriteTime") });
            gridView.Columns.Add(new GridViewColumn { Header = "Type", Width = 150, DisplayMemberBinding = new System.Windows.Data.Binding("Type") });
            gridView.Columns.Add(new GridViewColumn { Header = "Size", Width = 100, DisplayMemberBinding = new System.Windows.Data.Binding("FormattedSize") });
            FileListView.View = gridView;
        }

        private void ListViewButton_Click(object sender, RoutedEventArgs e)
        {
            DetailsViewButton.IsChecked = false;
            TilesViewButton.IsChecked = false;
            ListViewButton.IsChecked = true;

            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn { Header = "Name", Width = 700, DisplayMemberBinding = new System.Windows.Data.Binding("Name") });
            FileListView.View = gridView;
        }

        private void TilesViewButton_Click(object sender, RoutedEventArgs e)
        {
            DetailsViewButton.IsChecked = false;
            ListViewButton.IsChecked = false;
            TilesViewButton.IsChecked = true;

            FileListView.View = null; // This removes the GridView

            // Create an ItemsPanel for a wrap panel look
            var itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            FileListView.ItemsPanel = itemsPanelTemplate;

            // Set up item template (would be better in XAML)
            FileListView.ItemTemplate = new DataTemplate();
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            stackPanel.SetValue(StackPanel.WidthProperty, 120.0);
            stackPanel.SetValue(StackPanel.MarginProperty, new Thickness(5));

            FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Name"));
            textBlock.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            textBlock.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

            stackPanel.AppendChild(textBlock);
            FileListView.ItemTemplate.VisualTree = stackPanel;
        }

        private void FileListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (FileListView.SelectedItem is FileSystemItem item)
                {
                    if (item.IsDirectory)
                    {
                        NavigateToPath(item.FullName);
                    }
                    else
                    {
                        OpenButton_Click(sender, e);
                    }
                }
            }
            else if (e.Key == Key.Back)
            {
                UpButton_Click(sender, e);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // If there's a file name in the text box but no selection, try to use that
            string fileName = FileNameTextBox.Text;
            if (!string.IsNullOrEmpty(fileName) && FileListView.SelectedItems.Count == 0)
            {
                string fullPath = Path.Combine(_currentPath, fileName);

                // Check if the file exists
                if (File.Exists(fullPath))
                {
                    SelectedFilePath = fullPath;
                    DialogResult = true;
                    return;
                }

                // For multiselect, we could check for wildcards like *.txt here
                // but for simplicity, we'll just show a message
                CustomMessageBox.Show("The specified file does not exist.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Normal selection processing
            if (FileListView.SelectedItems.Count > 0)
            {
                if (Multiselect)
                {
                    SelectedFilePaths = FileListView.SelectedItems.Cast<FileSystemItem>().Where(f => !f.IsDirectory).Select(f => f.FullName).ToArray();
                    if (SelectedFilePaths.Length > 0)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        CustomMessageBox.Show("Please select at least one file.", "No Files Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (FileListView.SelectedItem is FileSystemItem item && !item.IsDirectory)
                {
                    SelectedFilePath = item.FullName;
                    DialogResult = true;
                }
                else
                {
                    CustomMessageBox.Show("Please select a file.", "No File Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                CustomMessageBox.Show("Please select a file.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion
    }
}