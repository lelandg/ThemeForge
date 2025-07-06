using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Button = System.Windows.Controls.Button;
using UserControl = System.Windows.Controls.UserControl;

namespace ThemeForge.Themes.Dialogs
{
    public partial class BreadcrumbNavigation : UserControl
    {
        public event EventHandler<string> NavigationRequested;
        
        private string _currentPath;
        private FileDialogSettings Settings => FileDialogSettings.Instance;

        public BreadcrumbNavigation()
        {
            InitializeComponent();
            LoadPathDropdown();
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                UpdateBreadcrumb();
            }
        }

        private void UpdateBreadcrumb()
        {
            BreadcrumbPanel.Children.Clear();
            
            if (string.IsNullOrEmpty(_currentPath))
                return;

            try
            {
                // Handle special cases
                if (_currentPath == "This PC")
                {
                    var button = CreateBreadcrumbButton("This PC", "This PC");
                    BreadcrumbPanel.Children.Add(button);
                    return;
                }

                var pathParts = GetPathParts(_currentPath);
                
                for (int i = 0; i < pathParts.Count; i++)
                {
                    // Add separator if not the first item
                    if (i > 0)
                    {
                        var separator = new TextBlock { Style = (Style)FindResource("BreadcrumbSeparatorStyle") };
                        BreadcrumbPanel.Children.Add(separator);
                    }

                    var part = pathParts[i];
                    var button = CreateBreadcrumbButton(part.Name, part.Path);
                    BreadcrumbPanel.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                // Fallback to simple path display
                var button = CreateBreadcrumbButton(Path.GetFileName(_currentPath) ?? _currentPath, _currentPath);
                BreadcrumbPanel.Children.Add(button);
            }
        }

        private List<(string Name, string Path)> GetPathParts(string path)
        {
            var parts = new List<(string Name, string Path)>();
            
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                var directories = new List<DirectoryInfo>();
                
                // Build list from bottom to top
                var current = directoryInfo;
                while (current != null)
                {
                    directories.Add(current);
                    current = current.Parent;
                }
                
                // Reverse to get top to bottom
                directories.Reverse();
                
                foreach (var dir in directories)
                {
                    string name = dir.Name;
                    
                    // Handle root drives
                    if (string.IsNullOrEmpty(name) && dir.FullName.Length == 3)
                    {
                        name = dir.FullName; // e.g., "C:\"
                    }
                    else if (string.IsNullOrEmpty(name))
                    {
                        name = dir.FullName;
                    }
                    
                    parts.Add((name, dir.FullName));
                }
            }
            catch
            {
                // If parsing fails, just add the current path
                parts.Add((Path.GetFileName(path) ?? path, path));
            }
            
            return parts;
        }

        private Button CreateBreadcrumbButton(string text, string path)
        {
            var button = new Button
            {
                Content = text,
                Style = (Style)FindResource("BreadcrumbButtonStyle"),
                Tag = path,
                MaxWidth = 120,
                ToolTip = path
            };
            
            button.Click += BreadcrumbButton_Click;
            return button;
        }

        private void BreadcrumbButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string path)
            {
                NavigationRequested?.Invoke(this, path);
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            NavigationRequested?.Invoke(this, homePath);
        }

        private void LoadPathDropdown()
        {
            var items = new List<object>();
            
            // Add recent folders
            var recentFolders = Settings.NavigationHistory.Where(h => Directory.Exists(h)).Take(5).ToList();
            if (recentFolders.Any())
            {
                items.Add(new { Name = "— Recent —", Path = "" });
                foreach (var folder in recentFolders)
                {
                    items.Add(new { Name = Path.GetFileName(folder) ?? folder, Path = folder });
                }
            }
            
            // Add frequent folders
            var frequentFolders = Settings.FrequentFolders.Where(f => Directory.Exists(f)).Take(5).ToList();
            if (frequentFolders.Any())
            {
                if (items.Any()) items.Add(new { Name = "", Path = "" }); // Separator
                items.Add(new { Name = "— Frequent —", Path = "" });
                foreach (var folder in frequentFolders)
                {
                    items.Add(new { Name = Path.GetFileName(folder) ?? folder, Path = folder });
                }
            }
            
            PathDropdown.ItemsSource = items;
            PathDropdown.Visibility = items.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PathDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PathDropdown.SelectedItem != null)
            {
                var item = PathDropdown.SelectedItem;
                var path = item.GetType().GetProperty("Path")?.GetValue(item) as string;
                
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    NavigationRequested?.Invoke(this, path);
                }
                
                // Clear selection
                PathDropdown.SelectedItem = null;
            }
        }

        public void RefreshDropdown()
        {
            LoadPathDropdown();
        }
    }
}