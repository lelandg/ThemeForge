using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ThemeForge.Themes;
using ThemeForge.Themes.Dialogs;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace ThemeForge;

public class SampleItem
{
    public string? Name { get; set; }
    public int Value { get; set; }
    public string? Description { get; set; }
}

public partial class MainWindow : Window
{
    public ObservableCollection<SampleItem> SampleData { get; } = new();
    private BitmapImage? loadedImage;

    public MainWindow()
    {
        InitializeComponent();

        // Enable window dragging by clicking on the title bar
        this.MouseLeftButtonDown += (s, e) =>
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        };

        // Skip theme application in design mode to keep design-time resources
        if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        {
            // Initialize theme manager
            DataContext = ThemeManager.Current;

            // Ensure default theme is applied to the main window
            ThemeManager.Current.CurrentTheme = ThemeManager.Current.BuiltInThemes.First(t => t.Name == "Default");

            // Populate sample data
            SampleData.Add(new SampleItem { Name = "Alpha", Value = 1, Description = "First item" });
            SampleData.Add(new SampleItem { Name = "Beta", Value = 2, Description = "Second item" });
            SampleData.Add(new SampleItem { Name = "Gamma", Value = 3, Description = "Third item" });

            // Add ESC key handler
            PreviewKeyDown += Window_PreviewKeyDown;

            // Populate theme selector
            ThemeSelector.ItemsSource = ThemeManager.Current.BuiltInThemes.Concat(ThemeManager.Current.CustomThemes);

            // Set selected theme from settings if available
            if (!string.IsNullOrEmpty(App.Settings.ThemeName))
            {
                var savedTheme = ThemeManager.Current.BuiltInThemes
                    .Concat(ThemeManager.Current.CustomThemes)
                    .FirstOrDefault(t => t.Name == App.Settings.ThemeName);

                if (savedTheme != null)
                {
                    ThemeManager.Current.CurrentTheme = savedTheme;
                }
            }

            ThemeSelector.SelectedItem = ThemeManager.Current.CurrentTheme;

            // Restore window position and size if available
            if (App.Settings.WindowWidth > 0 && App.Settings.WindowHeight > 0)
            {
                Width = App.Settings.WindowWidth;
                Height = App.Settings.WindowHeight;
            }

            if (App.Settings.WindowX >= 0 && App.Settings.WindowY >= 0)
            {
                Left = App.Settings.WindowX;
                Top = App.Settings.WindowY;
            }

            UpdateCurrentThemeText();
        }
    }

    private void UpdateCurrentThemeText()
    {
        CurrentThemeText.Text = ThemeManager.Current.CurrentTheme.Name;
    }

    private void ApplyThemeResources(Theme theme)
    {
        // WindowTheme
        Application.Current.Resources["ButtonBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.WindowTheme.ButtonBackground).Color);
        Application.Current.Resources["ButtonForeground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.WindowTheme.ButtonForeground).Color);
        Application.Current.Resources["ButtonHoverBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.WindowTheme.ButtonHoverBackground).Color);
        Application.Current.Resources["ButtonPressedBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.WindowTheme.ButtonPressedBackground).Color);
        Application.Current.Resources["ControlBorderBrush"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.WindowTheme.ControlBorderBrush).Color);
        // MessageBoxTheme (if you use these keys elsewhere)
        Application.Current.Resources["MessageBoxButtonBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.MessageBoxTheme.ButtonBackground).Color);
        Application.Current.Resources["MessageBoxButtonForeground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.MessageBoxTheme.ButtonForeground).Color);
        Application.Current.Resources["MessageBoxButtonHoverBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.MessageBoxTheme.ButtonHoverBackground).Color);
        Application.Current.Resources["MessageBoxButtonPressedBackground"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.MessageBoxTheme.ButtonPressedBackground).Color);
        Application.Current.Resources["MessageBoxButtonOutline"] = new System.Windows.Media.SolidColorBrush(((System.Windows.Media.SolidColorBrush)theme.MessageBoxTheme.ButtonOutline).Color);
    }

    private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeSelector.SelectedItem is Theme selectedTheme)
        {
            ThemeManager.Current.CurrentTheme = selectedTheme;
            ApplyThemeResources(selectedTheme);
            UpdateCurrentThemeText();

            // Save current theme to settings
            App.Settings.ThemeName = selectedTheme.Name;
        }
    }

    private void OpenThemeManager_Click(object sender, RoutedEventArgs e)
    {
        var themeManager = new ThemeManagerWindow();
        themeManager.Owner = this;
        themeManager.ShowDialog();

        // Refresh theme selector after dialog closes
        ThemeSelector.ItemsSource = null;
        ThemeSelector.ItemsSource = ThemeManager.Current.BuiltInThemes.Concat(ThemeManager.Current.CustomThemes);
        ThemeSelector.SelectedItem = ThemeManager.Current.CurrentTheme;
        ApplyThemeResources(ThemeManager.Current.CurrentTheme);
        UpdateCurrentThemeText();

        // Save current theme to settings
        App.Settings.ThemeName = ThemeManager.Current.CurrentTheme.Name;
    }

    private void ShowMessageBox_Click(object sender, RoutedEventArgs e)
    {
        CustomMessageBox.Show("This is a sample message", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ShowQuestion_Click(object sender, RoutedEventArgs e)
    {
        var result = CustomMessageBox.Show("Would you like to create a new theme?", 
            "Question", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var dialog = new TextInputDialog("Enter a name for the new theme:", "New Theme");
            dialog.Owner = this;

            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
            {
                var newTheme = ThemeManager.Current.CreateNewThemeBasedOnCurrent(dialog.InputText);
                ThemeManager.Current.CurrentTheme = newTheme;
                ThemeSelector.ItemsSource = null;
                ThemeSelector.ItemsSource = ThemeManager.Current.BuiltInThemes.Concat(ThemeManager.Current.CustomThemes);
                ThemeSelector.SelectedItem = newTheme;
                ApplyThemeResources(newTheme);
                UpdateCurrentThemeText();


                // Save current theme to settings
                App.Settings.ThemeName = newTheme.Name;
            }
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        // Save window position and size
        App.Settings.WindowWidth = Width;
        App.Settings.WindowHeight = Height;
        App.Settings.WindowX = Left;
        App.Settings.WindowY = Top;

        // Save current theme
        if (ThemeManager.Current.CurrentTheme != null)
        {
            App.Settings.ThemeName = ThemeManager.Current.CurrentTheme.Name;
        }
    }

    private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        switch (e.Key)
        {
            case System.Windows.Input.Key.Escape:
                var result = CustomMessageBox.Show("Are you sure you want to quit?", 
                    "Confirm Exit", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
                break;
            case Key.O:
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    OpenImage_Click(sender, e);
                }
                break;
            case Key.S:
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    SaveImage_Click(sender, e);
                }
                break;
        }
    }

    private void OpenImage_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new CustomOpenFileDialog
        {
            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Title = "Open Image"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                loadedImage = new BitmapImage();
                loadedImage.BeginInit();
                loadedImage.UriSource = new Uri(openFileDialog.SelectedFilePath);
                loadedImage.CacheOption = BitmapCacheOption.OnLoad;
                loadedImage.EndInit();

                // Show the image viewer window
                var imageViewer = new ImageViewerWindow(loadedImage);
                imageViewer.Owner = this;
                imageViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveImage_Click(object sender, RoutedEventArgs e)
    {
        if (loadedImage == null)
        {
            CustomMessageBox.Show("No image is currently loaded.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var saveFileDialog = new CustomSaveFileDialog
        {
            Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|All files (*.*)|*.*",
            DefaultFileName = ".png",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(loadedImage));
                    encoder.Save(fileStream);
                }

                CustomMessageBox.Show("Image saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void Close_Window(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void ChooseColor_Click(object sender, RoutedEventArgs e)
    {
        var colorPicker = new ColorPickerDialog(Colors.Blue)
        {
            ColorPickerTitle =
            {
                Text = $"Demo dialog (does nothing)"
            },
            Owner = this
        };
        colorPicker.ShowDialog();
    }
}
