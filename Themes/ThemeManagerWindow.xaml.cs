using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ThemeForge.Themes.Dialogs;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ThemeForge.Themes
{
    public partial class ThemeManagerWindow : Window
    {
        private Theme _workingTheme;

        public ThemeManagerWindow()
        {
            InitializeComponent();
            DataContext = ThemeManager.Current;
            PreviewKeyDown += Window_PreviewKeyDown;

            // Enable window dragging by clicking on the title bar
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };

            // Create a working copy of the current theme
            _workingTheme = ThemeManager.Current.CurrentTheme;

            // Populate theme selector
            RefreshThemeSelector();
        }

        private void RefreshThemeSelector()
        {
            ThemeSelector.ItemsSource = null;
            var themes = ThemeManager.Current.BuiltInThemes.Concat(ThemeManager.Current.CustomThemes).ToList();
            ThemeSelector.ItemsSource = themes;
            ThemeSelector.SelectedItem = _workingTheme;
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is Theme selectedTheme)
            {
                _workingTheme = selectedTheme;

                // Update delete button state
                DeleteThemeButton.IsEnabled = !selectedTheme.IsBuiltIn;

                // Apply the theme changes to this window immediately for a live preview
                ApplyWorkingThemeToWindowAndRefreshBindings();
                
                // Also apply the theme globally so all windows get updated
                ThemeManager.Current.CurrentTheme = selectedTheme;
                ThemeManager.Current.ApplyTheme();
            }
        }
        
        private void NewThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TextInputDialog("Enter a name for the new theme:", "New Theme");
            dialog.Owner = this;

            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
            {
                var newTheme = ThemeManager.Current.CreateNewThemeBasedOnCurrent(dialog.InputText);
                _workingTheme = newTheme;
                RefreshThemeSelector(); // This will trigger the preview via SelectionChanged
            }
        }
        
        private void DeleteThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is Theme selectedTheme && !selectedTheme.IsBuiltIn)
            {
                if (CustomMessageBox.Show("Are you sure you want to delete this theme?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Remove from custom themes
                    ThemeManager.Current.CustomThemes.Remove(selectedTheme);
                    ThemeManager.Current.SaveCustomThemes();

                    // Save current theme to settings (this line is duplicated)
                    App.Settings.ThemeName = ThemeManager.Current.CurrentTheme.Name;

                    // Set current theme to default if we're deleting the active theme
                    if (ThemeManager.Current.CurrentTheme == selectedTheme)
                    {
                        _workingTheme = ThemeManager.Current.BuiltInThemes[0];
                        ThemeManager.Current.CurrentTheme = _workingTheme;
                    }
                    else
                    {
                        _workingTheme = ThemeManager.Current.CurrentTheme;
                    }

                    RefreshThemeSelector();
                }
            }
        }
        
        private void ApplyThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_workingTheme != null)
            {
                ThemeManager.Current.CurrentTheme = _workingTheme;
                // Force main window to refresh theme
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Resources.MergedDictionaries.Clear();
                    foreach (var dict in System.Windows.Application.Current.Resources.MergedDictionaries)
                    {
                        mainWindow.Resources.MergedDictionaries.Add(dict);
                    }
                    mainWindow.InvalidateVisual();
                    mainWindow.UpdateLayout();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExportThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_workingTheme == null) return;

            var dialog = new SaveFileDialog
            {
                Filter = "Theme Files (*.json)|*.json",
                FileName = $"{_workingTheme.Name}.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ThemeManager.Current.ExportTheme(_workingTheme, dialog.FileName, ThemeExportType.Both);
                    CustomMessageBox.Show("Theme exported successfully.", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show($"Error exporting theme: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Theme Files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var importedTheme = ThemeManager.Current.ImportTheme(dialog.FileName);
                    _workingTheme = importedTheme;
                    RefreshThemeSelector(); // This will trigger the preview via SelectionChanged

                    CustomMessageBox.Show("Theme imported successfully.", "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show($"Error importing theme: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        #region Color Editors

        /// <summary>
        /// Applies the in-memory _workingTheme to this window's resources and updates its DataContext
        /// to provide an immediate visual preview of changes.
        /// </summary>
        private void ApplyWorkingThemeToWindowAndRefreshBindings()
        {
            // Part 1: Update the window's own styling to reflect _workingTheme.
            UpdateWindowResourcesFromWorkingTheme();

            // Part 2: Update the DataContext so that UI elements bound to theme properties
            // (like color swatches) show the values from _workingTheme.
            // We create an anonymous object with a 'CurrentTheme' property to match the original binding path.
            DataContext = new { CurrentTheme = _workingTheme };

            // Force the UI to re-render with the new resources and data context.
            InvalidateVisual();
            UpdateLayout();
        }

        /// <summary>
        /// Manually updates this window's local resources from the _workingTheme object.
        /// This overrides the application-level theme for this window only, by mapping
        /// theme properties to the corresponding resource keys used in XAML styles.
        /// </summary>
        private void UpdateWindowResourcesFromWorkingTheme()
        {
            // Window Theme
            Resources["MainBackground"] = _workingTheme.WindowTheme.MainBackground;
            Resources["TitleBarBackground"] = _workingTheme.WindowTheme.TitleBarBackground;
            Resources["TextForeground"] = _workingTheme.WindowTheme.TextForeground;
            Resources["LabelForeground"] = _workingTheme.WindowTheme.LabelForeground;
            Resources["MenuBackground"] = _workingTheme.WindowTheme.MenuBackground;
            Resources["MenuForeground"] = _workingTheme.WindowTheme.MenuForeground;
            Resources["MainAccent"] = _workingTheme.WindowTheme.MainAccent;
            Resources["AlternateTextForeground"] = _workingTheme.WindowTheme.AlternateTextForeground;
            Resources["SplitterBackground"] = _workingTheme.WindowTheme.SplitterBackground;
            Resources["GroupBoxBorder"] = _workingTheme.WindowTheme.GroupBoxBorder;
            Resources["PanelBackground"] = _workingTheme.WindowTheme.PanelBackground;
            Resources["ControlBackground"] = _workingTheme.WindowTheme.ControlBackground;
            Resources["ComboBoxBackground"] = _workingTheme.WindowTheme.ComboBoxBackground;
            Resources["ComboBoxItemBackground"] = _workingTheme.WindowTheme.ComboBoxItemBackground;
            Resources["ComboBoxItemHoverBackground"] = _workingTheme.WindowTheme.ComboBoxItemHoverBackground;
            Resources["ComboBoxItemSelectedBackground"] = _workingTheme.WindowTheme.ComboBoxItemSelectedBackground;
            Resources["ControlBorderBrush"] = _workingTheme.WindowTheme.ControlBorderBrush;
            Resources["ControlHoverBackground"] = _workingTheme.WindowTheme.ControlHoverBackground;
            Resources["ButtonBackground"] = _workingTheme.WindowTheme.ButtonBackground;
            Resources["ButtonHoverBackground"] = _workingTheme.WindowTheme.ButtonHoverBackground;
            Resources["ButtonPressedBackground"] = _workingTheme.WindowTheme.ButtonPressedBackground;
            Resources["ButtonForeground"] = _workingTheme.WindowTheme.ButtonForeground;

            // MessageBox Theme
            Resources["MsgBoxWindowBackground"] = _workingTheme.MessageBoxTheme.WindowBackground;
            Resources["MsgBoxTitleBackground"] = _workingTheme.MessageBoxTheme.TitleBackground;
            Resources["MsgBoxTitleForeground"] = _workingTheme.MessageBoxTheme.TitleForeground;
            Resources["MsgBoxBorderBrush"] = _workingTheme.MessageBoxTheme.BorderBrush;
            Resources["MsgBoxButtonBackground"] = _workingTheme.MessageBoxTheme.ButtonBackground;
            Resources["MsgBoxButtonHoverBackground"] = _workingTheme.MessageBoxTheme.ButtonHoverBackground;
            Resources["MsgBoxButtonPressedBackground"] = _workingTheme.MessageBoxTheme.ButtonPressedBackground;
            Resources["MsgBoxButtonForeground"] = _workingTheme.MessageBoxTheme.ButtonForeground;
            Resources["MsgBoxButtonDisabledBackground"] = _workingTheme.MessageBoxTheme.ButtonDisabledBackground;
            Resources["MsgBoxButtonDisabledForeground"] = _workingTheme.MessageBoxTheme.ButtonDisabledForeground;
            Resources["MsgBoxButtonOutline"] = _workingTheme.MessageBoxTheme.ButtonOutline;
        }

        private void EditMainBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Main Background", _workingTheme.WindowTheme.MainBackground, newBrush => _workingTheme.WindowTheme.MainBackground = newBrush);
        }

        private void EditTitleBarBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Title Bar Background", _workingTheme.WindowTheme.TitleBarBackground, newBrush => _workingTheme.WindowTheme.TitleBarBackground = newBrush);
        }

        private void EditTextForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Text Foreground", _workingTheme.WindowTheme.TextForeground, newBrush => _workingTheme.WindowTheme.TextForeground = newBrush);
        }

        private void EditLabelForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Label Foreground", _workingTheme.WindowTheme.LabelForeground, newBrush => _workingTheme.WindowTheme.LabelForeground = newBrush);
        }

        private void EditGroupBoxBorder_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("GroupBox Border", _workingTheme.WindowTheme.GroupBoxBorder, newBrush => _workingTheme.WindowTheme.GroupBoxBorder = newBrush);
        }

        private void EditMainAccent_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Main Accent", _workingTheme.WindowTheme.MainAccent, newBrush => _workingTheme.WindowTheme.MainAccent = newBrush);
        }

        private void EditMenuBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Menu Background", _workingTheme.WindowTheme.MenuBackground, newBrush => _workingTheme.WindowTheme.MenuBackground = newBrush);
        }

        private void EditMenuForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Menu Foreground", _workingTheme.WindowTheme.MenuForeground, newBrush => _workingTheme.WindowTheme.MenuForeground = newBrush);
        }

        private void EditAlternateTextForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Alternate Text Foreground", _workingTheme.WindowTheme.AlternateTextForeground, newBrush => _workingTheme.WindowTheme.AlternateTextForeground = newBrush);
        }

        private void EditSplitterBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Splitter Background", _workingTheme.WindowTheme.SplitterBackground, newBrush => _workingTheme.WindowTheme.SplitterBackground = newBrush);
        }

        private void EditPanelBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Panel Background", _workingTheme.WindowTheme.PanelBackground, newBrush => _workingTheme.WindowTheme.PanelBackground = newBrush);
        }

        private void EditControlBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Control Background", _workingTheme.WindowTheme.ControlBackground, newBrush => _workingTheme.WindowTheme.ControlBackground = newBrush);
        }

        private void EditControlBorderBrush_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Control Border", _workingTheme.WindowTheme.ControlBorderBrush, newBrush => _workingTheme.WindowTheme.ControlBorderBrush = newBrush);
        }

        private void EditControlHoverBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Control Hover Background", _workingTheme.WindowTheme.ControlHoverBackground, newBrush => _workingTheme.WindowTheme.ControlHoverBackground = newBrush);
        }

        private void EditButtonBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Button Background", _workingTheme.WindowTheme.ButtonBackground, newBrush => _workingTheme.WindowTheme.ButtonBackground = newBrush);
        }

        private void EditButtonHoverBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Button Hover Background", _workingTheme.WindowTheme.ButtonHoverBackground, newBrush => _workingTheme.WindowTheme.ButtonHoverBackground = newBrush);
        }

        private void EditButtonPressedBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Button Pressed Background", _workingTheme.WindowTheme.ButtonPressedBackground, newBrush => _workingTheme.WindowTheme.ButtonPressedBackground = newBrush);
        }

        private void EditButtonForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Button Foreground", _workingTheme.WindowTheme.ButtonForeground, newBrush => _workingTheme.WindowTheme.ButtonForeground = newBrush);
        }

        private void EditMsgBoxWindowBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Window Background", _workingTheme.MessageBoxTheme.WindowBackground, newBrush => _workingTheme.MessageBoxTheme.WindowBackground = newBrush);
        }

        private void EditMsgBoxTitleBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Title Background", _workingTheme.MessageBoxTheme.TitleBackground, newBrush => _workingTheme.MessageBoxTheme.TitleBackground = newBrush);
        }

        private void EditMsgBoxBorderBrush_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Border Brush", _workingTheme.MessageBoxTheme.BorderBrush, newBrush => _workingTheme.MessageBoxTheme.BorderBrush = newBrush);
        }

        private void EditMsgBoxButtonBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Background", _workingTheme.MessageBoxTheme.ButtonBackground, newBrush => _workingTheme.MessageBoxTheme.ButtonBackground = newBrush);
        }

        private void EditMsgBoxButtonHoverBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Hover Background", _workingTheme.MessageBoxTheme.ButtonHoverBackground, newBrush => _workingTheme.MessageBoxTheme.ButtonHoverBackground = newBrush);
        }

        private void EditMsgBoxButtonPressedBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Pressed Background", _workingTheme.MessageBoxTheme.ButtonPressedBackground, newBrush => _workingTheme.MessageBoxTheme.ButtonPressedBackground = newBrush);
        }

        private void EditMsgBoxButtonForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Foreground", _workingTheme.MessageBoxTheme.ButtonForeground, newBrush => _workingTheme.MessageBoxTheme.ButtonForeground = newBrush);
        }

        private void EditMsgBoxTitleForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Title Foreground", _workingTheme.MessageBoxTheme.TitleForeground, newBrush => _workingTheme.MessageBoxTheme.TitleForeground = newBrush);
        }

        private void EditMsgBoxButtonDisabledBackground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Disabled Background", _workingTheme.MessageBoxTheme.ButtonDisabledBackground, newBrush => _workingTheme.MessageBoxTheme.ButtonDisabledBackground = newBrush);
        }

        private void EditMsgBoxButtonDisabledForeground_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Disabled Foreground", _workingTheme.MessageBoxTheme.ButtonDisabledForeground, newBrush => _workingTheme.MessageBoxTheme.ButtonDisabledForeground = newBrush);
        }

        private void EditMsgBoxButtonOutline_Click(object sender, RoutedEventArgs e)
        {
            EditBrush("Message Box Button Outline", _workingTheme.MessageBoxTheme.ButtonOutline, newBrush => _workingTheme.MessageBoxTheme.ButtonOutline = newBrush);
        }

        private void EditBrush(string name, Brush currentBrush, Action<SolidColorBrush> setter)
        {
            if (currentBrush is SolidColorBrush solidBrush)
            {
                var colorPicker = new ColorPickerDialog(solidBrush.Color);
                colorPicker.Title = $"Select {name}";
                if (colorPicker.ShowDialog() == true)
                {
                    var newColor = colorPicker.SelectedColor;
                    setter(new SolidColorBrush(newColor));

                    // Apply the theme changes to this window immediately for a live preview
                    ApplyWorkingThemeToWindowAndRefreshBindings();
                }
            }
        }

        #endregion
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}