using ThemeForge.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UserControl = System.Windows.Controls.UserControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace ThemeForge.Themes.Dialogs
{
    public partial class SmartFileNameBox : UserControl
    {
        public event EventHandler<string> FileSelected;
        public event EventHandler EnterPressed;
        
        private DispatcherTimer _suggestionTimer;
        private string _currentDirectory;
        private string[] _allowedExtensions;
        private bool _isUpdatingProgrammatically;

        public SmartFileNameBox()
        {
            InitializeComponent();
            
            _suggestionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _suggestionTimer.Tick += SuggestionTimer_Tick;
            
            // Subscribe to TextChanged event programmatically
            FileNameComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, 
                new TextChangedEventHandler(FileNameComboBox_TextChanged));
        }

        public string Text
        {
            get => FileNameComboBox.Text;
            set
            {
                _isUpdatingProgrammatically = true;
                FileNameComboBox.Text = value ?? "";
                _isUpdatingProgrammatically = false;
            }
        }

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;
                UpdateRecentFiles();
            }
        }

        public string[] AllowedExtensions
        {
            get => _allowedExtensions;
            set => _allowedExtensions = value;
        }

        public bool HasFocus => FileNameComboBox.IsFocused || FileNameComboBox.IsKeyboardFocusWithin;

        private void UpdateRecentFiles()
        {
            try
            {
                var items = new List<string>();
                
                // Add recent files that match current directory or filter
                var settings = FileDialogSettings.Instance;
                var recentFiles = settings.RecentFiles.Where(f => File.Exists(f));
                
                if (!string.IsNullOrEmpty(_currentDirectory))
                {
                    // Prioritize files from current directory
                    var currentDirFiles = recentFiles
                        .Where(f => Path.GetDirectoryName(f)?.Equals(_currentDirectory, StringComparison.OrdinalIgnoreCase) == true)
                        .Select(f => Path.GetFileName(f));
                    items.AddRange(currentDirFiles);
                }
                
                // Add other recent files (just filenames)
                var otherRecentFiles = recentFiles
                    .Where(f => !items.Contains(Path.GetFileName(f)))
                    .Select(f => Path.GetFileName(f))
                    .Take(10 - items.Count);
                items.AddRange(otherRecentFiles);

                // Filter by allowed extensions if specified
                if (_allowedExtensions != null && _allowedExtensions.Length > 0)
                {
                    items = items.Where(f => _allowedExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                }

                FileNameComboBox.ItemsSource = items.Take(10).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating recent files: {ex.Message}");
            }
        }

        private void FileNameComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingProgrammatically)
                return;

            // Reset timer for autocomplete suggestions
            _suggestionTimer.Stop();
            _suggestionTimer.Start();
        }

        private void SuggestionTimer_Tick(object sender, EventArgs e)
        {
            _suggestionTimer.Stop();
            UpdateSuggestions();
        }

        private void UpdateSuggestions()
        {
            try
            {
                var text = FileNameComboBox.Text;
                if (string.IsNullOrEmpty(text) || text.Length < 1 || string.IsNullOrEmpty(_currentDirectory))
                    return;

                var suggestions = ExplorerIntegration.GetFileSuggestions(_currentDirectory, text, _allowedExtensions, 8);
                
                if (suggestions.Any())
                {
                    _isUpdatingProgrammatically = true;
                    var currentText = FileNameComboBox.Text;
                    
                    FileNameComboBox.ItemsSource = suggestions;
                    FileNameComboBox.IsDropDownOpen = true;
                    
                    // Restore text and cursor position
                    FileNameComboBox.Text = currentText;
                    if (FileNameComboBox.Template.FindName("PART_EditableTextBox", FileNameComboBox) is TextBox textBox)
                    {
                        textBox.CaretIndex = currentText.Length;
                    }
                    
                    _isUpdatingProgrammatically = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating file suggestions: {ex.Message}");
            }
        }

        private void FileNameComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnterPressed?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                FileNameComboBox.IsDropDownOpen = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && FileNameComboBox.IsDropDownOpen && FileNameComboBox.Items.Count > 0)
            {
                // Auto-complete with first suggestion
                var firstSuggestion = FileNameComboBox.Items[0] as string;
                if (!string.IsNullOrEmpty(firstSuggestion))
                {
                    _isUpdatingProgrammatically = true;
                    FileNameComboBox.Text = firstSuggestion;
                    FileNameComboBox.IsDropDownOpen = false;
                    _isUpdatingProgrammatically = false;
                }
                e.Handled = true;
            }
        }

        private void FileNameComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Small delay to allow item selection to complete
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FileNameComboBox.IsDropDownOpen = false;
            }), DispatcherPriority.Background);
        }

        private void FileNameComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when focused
            if (FileNameComboBox.Template.FindName("PART_EditableTextBox", FileNameComboBox) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void FileNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingProgrammatically)
                return;

            if (FileNameComboBox.SelectedItem is string selectedFile)
            {
                _isUpdatingProgrammatically = true;
                FileNameComboBox.Text = selectedFile;
                FileNameComboBox.IsDropDownOpen = false;
                _isUpdatingProgrammatically = false;
                
                FileSelected?.Invoke(this, selectedFile);
            }
        }

        public new void Focus()
        {
            FileNameComboBox.Focus();
        }

        public void SelectAll()
        {
            if (FileNameComboBox.Template.FindName("PART_EditableTextBox", FileNameComboBox) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }
    }
}