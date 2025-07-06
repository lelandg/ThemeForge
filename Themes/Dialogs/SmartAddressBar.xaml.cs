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
    public partial class SmartAddressBar : UserControl
    {
        public event EventHandler<string> NavigationRequested;
        
        private DispatcherTimer _suggestionTimer;
        private string _currentPath;
        private bool _isUpdatingProgrammatically;

        public SmartAddressBar()
        {
            InitializeComponent();
            
            _suggestionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _suggestionTimer.Tick += SuggestionTimer_Tick;
            
            // Subscribe to TextChanged event programmatically
            AddressComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, 
                new TextChangedEventHandler(AddressComboBox_TextChanged));
            
            LoadAddressHistory();
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                _isUpdatingProgrammatically = true;
                AddressComboBox.Text = value ?? "";
                _isUpdatingProgrammatically = false;
            }
        }

        private void LoadAddressHistory()
        {
            try
            {
                var items = new List<string>();
                
                // Add Explorer recent folders
                var explorerHistory = ExplorerIntegration.GetExplorerRecentFolders();
                items.AddRange(explorerHistory);
                
                // Add our own navigation history
                var settings = FileDialogSettings.Instance;
                var ourHistory = settings.NavigationHistory.Where(h => Directory.Exists(h) && !items.Contains(h));
                items.AddRange(ourHistory);
                
                // Add common system folders
                var commonFolders = new[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")
                }.Where(f => Directory.Exists(f) && !items.Contains(f));
                
                items.AddRange(commonFolders);
                
                AddressComboBox.ItemsSource = items.Take(20).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading address history: {ex.Message}");
            }
        }

        private void AddressComboBox_TextChanged(object sender, TextChangedEventArgs e)
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
                var text = AddressComboBox.Text;
                if (string.IsNullOrEmpty(text) || text.Length < 2)
                    return;

                var suggestions = ExplorerIntegration.GetFolderSuggestions(text, 10);
                
                if (suggestions.Any())
                {
                    _isUpdatingProgrammatically = true;
                    var currentText = AddressComboBox.Text;
                    
                    AddressComboBox.ItemsSource = suggestions;
                    AddressComboBox.IsDropDownOpen = true;
                    
                    // Restore text and cursor position
                    AddressComboBox.Text = currentText;
                    if (AddressComboBox.Template.FindName("PART_EditableTextBox", AddressComboBox) is TextBox textBox)
                    {
                        textBox.CaretIndex = currentText.Length;
                    }
                    
                    _isUpdatingProgrammatically = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating suggestions: {ex.Message}");
            }
        }

        private void AddressComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToAddress();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                AddressComboBox.IsDropDownOpen = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && AddressComboBox.IsDropDownOpen && AddressComboBox.Items.Count > 0)
            {
                // Auto-complete with first suggestion
                var firstSuggestion = AddressComboBox.Items[0] as string;
                if (!string.IsNullOrEmpty(firstSuggestion))
                {
                    _isUpdatingProgrammatically = true;
                    AddressComboBox.Text = firstSuggestion;
                    AddressComboBox.IsDropDownOpen = false;
                    _isUpdatingProgrammatically = false;
                }
                e.Handled = true;
            }
        }

        private void AddressComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Small delay to allow item selection to complete
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AddressComboBox.IsDropDownOpen = false;
            }), DispatcherPriority.Background);
        }

        private void AddressComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Select all text when focused
            if (AddressComboBox.Template.FindName("PART_EditableTextBox", AddressComboBox) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void AddressComboBox_DropDownOpened(object sender, EventArgs e)
        {
            // Refresh history when dropdown opens
            if (AddressComboBox.ItemsSource == null || !AddressComboBox.ItemsSource.Cast<object>().Any())
            {
                LoadAddressHistory();
            }
        }

        private void AddressComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingProgrammatically)
                return;

            if (AddressComboBox.SelectedItem is string selectedPath)
            {
                _isUpdatingProgrammatically = true;
                AddressComboBox.Text = selectedPath;
                AddressComboBox.IsDropDownOpen = false;
                _isUpdatingProgrammatically = false;
                
                NavigateToAddress();
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToAddress();
        }

        private void NavigateToAddress()
        {
            var address = AddressComboBox.Text?.Trim();
            if (string.IsNullOrEmpty(address))
                return;

            try
            {
                // Add to Explorer history
                if (Directory.Exists(address))
                {
                    ExplorerIntegration.AddToExplorerHistory(address);
                    NavigationRequested?.Invoke(this, address);
                }
                else
                {
                    CustomMessageBox.Show($"The path '{address}' does not exist.", "Invalid Path", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error navigating to '{address}': {ex.Message}", "Navigation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshHistory()
        {
            LoadAddressHistory();
        }
    }
}