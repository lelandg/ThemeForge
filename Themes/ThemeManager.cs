using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using GroupBox = System.Windows.Controls.GroupBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace ThemeForge.Themes;

/// <summary>
/// Manages application-wide theming, including windows and message boxes
/// </summary>
public class ThemeManager : INotifyPropertyChanged
{
    private static ThemeManager? _instance;

    /// <summary>
    /// Gets the current theme manager instance or creates a new one with default settings
    /// </summary>
    public static ThemeManager Current => _instance ??= new ThemeManager();

    /// <summary>
    /// Event raised when the active theme changes
    /// </summary>
    public event EventHandler? ThemeChanged;

    private Theme _currentTheme;

    /// <summary>
    /// The currently active theme
    /// </summary>
    public Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                ApplyTheme();
                ThemeChanged?.Invoke(this, EventArgs.Empty);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
            }
        }
    }

    /// <summary>
    /// Shows a color picker dialog and returns the selected color
    /// </summary>
    /// <param name="initialColor">The initial color to display</param>
    /// <returns>The selected color, or null if canceled</returns>
    public System.Windows.Media.Color? ShowColorPicker(System.Windows.Media.Color initialColor)
    {
        var dialog = new Dialogs.ColorPickerDialog(initialColor);
        if (dialog.ShowDialog() == true)
        {
            return dialog.SelectedColor;
        }
        return null;
    }

    /// <summary>
    /// Collection of built-in themes
    /// </summary>
    public List<Theme> BuiltInThemes { get; } = new List<Theme>();

    /// <summary>
    /// Collection of custom (user-created) themes
    /// </summary>
    public List<Theme> CustomThemes { get; } = new List<Theme>();

    private ThemeManager()
    {
        // Initialize with the default theme
        _currentTheme = CreateDefaultTheme();

        // Create built-in themes
        InitializeBuiltInThemes();

        // Ensure themes directory exists
        EnsureThemesDirectoryExists();

        // Load any saved custom themes
        LoadCustomThemes();
    }

    /// <summary>
    /// Ensures that the directory for storing themes exists
    /// </summary>
    private void EnsureThemesDirectoryExists()
    {
        string themesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeForge", "Themes");
        Directory.CreateDirectory(themesFolder);
    }

    /// <summary>
    /// Initialize the collection of built-in themes
    /// </summary>
    private void InitializeBuiltInThemes()
    {
        // Add default theme
        BuiltInThemes.Add(CreateDefaultTheme());

        // Add dark themes
        BuiltInThemes.Add(CreateDarkBlueTheme());
        BuiltInThemes.Add(CreateDarkPurpleTheme());
        BuiltInThemes.Add(CreateDarkGreenTheme());

        // Add light themes
        BuiltInThemes.Add(CreateLightBlueTheme());
        BuiltInThemes.Add(CreateLightGreenTheme());
        BuiltInThemes.Add(CreateLightOrangeTheme());
    }

    /// <summary>
    /// Creates the default theme based on the main window style
    /// </summary>
    private Theme CreateDefaultTheme()
    {
        return new Theme
        {
            Name = "Default",
            IsBuiltIn = true,
            IsDarkTheme = false,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE))
            }
        };
    }

    private Theme CreateDarkBlueTheme()
    {
        return new Theme
        {
            Name = "Dark Blue",
            IsBuiltIn = true,
            IsDarkTheme = true,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x2E)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Colors.White),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x5A)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x2C, 0x3E, 0x50)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x34, 0x49, 0x5E)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x1C, 0x2E, 0x40)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x38)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x77)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x48)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0x7F, 0xA9, 0xD8)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x38)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x48)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x2E)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x2C, 0x3E, 0x50)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x34, 0x49, 0x5E)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x1C, 0x2E, 0x40)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x55)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x77))
            }
        };
    }

    private Theme CreateDarkPurpleTheme()
    {
        return new Theme
        {
            Name = "Dark Purple",
            IsBuiltIn = true,
            IsDarkTheme = true,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0x2D, 0x1E, 0x3E)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Colors.White),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0x55, 0x3A, 0x7A)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x64, 0x3A, 0x7A)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x7A, 0x4A, 0x8A)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x54, 0x2A, 0x6A)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0x33, 0x24, 0x44)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x3A, 0x7A)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0x43, 0x34, 0x54)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0xC4, 0x9D, 0xE3)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0x33, 0x24, 0x44)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0x43, 0x34, 0x54)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0x2D, 0x1E, 0x3E)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x42, 0x25, 0x6F)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x64, 0x3A, 0x7A)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x7A, 0x4A, 0x8A)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x54, 0x2A, 0x6A)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x55)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x77))
            }
        };
    }

    private Theme CreateDarkGreenTheme()
    {
        return new Theme
        {
            Name = "Dark Green",
            IsBuiltIn = true,
            IsDarkTheme = true,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x3E, 0x2D)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Colors.White),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0x3A, 0x7A, 0x55)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3A, 0x7A, 0x64)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x4A, 0x8A, 0x7A)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2A, 0x6A, 0x54)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0x24, 0x44, 0x33)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x77, 0x55)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0x34, 0x54, 0x43)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0x9D, 0xE3, 0xC4)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0x24, 0x44, 0x33)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0x34, 0x54, 0x43)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x3E, 0x2D)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x6F, 0x42)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3A, 0x7A, 0x64)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x4A, 0x8A, 0x7A)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2A, 0x6A, 0x54)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0x44, 0x55, 0x44)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x88, 0x99, 0x88)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0x55, 0x77, 0x55))
            }
        };
    }

    private Theme CreateLightBlueTheme()
    {
        return new Theme
        {
            Name = "Light Blue",
            IsBuiltIn = true,
            IsDarkTheme = false,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0xF0, 0xFA)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x0E, 0x4A, 0x8C)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0xF0, 0xFA)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x1E, 0x65, 0xA6)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE))
            }
        };
    }

    private Theme CreateLightGreenTheme()
    {
        return new Theme
        {
            Name = "Light Green",
            IsBuiltIn = true,
            IsDarkTheme = false,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xFA, 0xF5)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0xB0, 0xDE, 0xC4)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0xD6, 0x95)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0xB6, 0x75)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xFF, 0xF5)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0xDE, 0xC4)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xEF, 0xE5)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0x15, 0x6B, 0x40)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xFF, 0xF5)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xEF, 0xE5)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xFA, 0xF5)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0xC6, 0x85)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0xD6, 0x95)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0xB6, 0x75)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0xB0, 0xDE, 0xC4))
            }
        };
    }

    private Theme CreateLightOrangeTheme()
    {
        return new Theme
        {
            Name = "Light Orange",
            IsBuiltIn = true,
            IsDarkTheme = false,
            WindowTheme = new WindowTheme
            {
                MainBackground = new SolidColorBrush(Color.FromRgb(0xFA, 0xF5, 0xF0)),
                TitleBarBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                MainAccent = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                MenuBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                MenuForeground = new SolidColorBrush(Colors.White),
                AlternateTextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                SplitterBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                GroupBoxBorder = new SolidColorBrush(Color.FromRgb(0xDE, 0xC4, 0xB0)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0xF6, 0x95, 0x4E)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0xD6, 0x75, 0x2E)),
                ButtonForeground = new SolidColorBrush(Colors.White),
                PanelBackground = new SolidColorBrush(Colors.Transparent),
                ControlBackground = new SolidColorBrush(Color.FromRgb(0xFF, 0xF5, 0xE6)),
                ControlBorderBrush = new SolidColorBrush(Color.FromRgb(0xDE, 0xC4, 0xB0)),
                ControlHoverBackground = new SolidColorBrush(Color.FromRgb(0xEF, 0xE5, 0xD6)),
                TextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                LabelForeground = new SolidColorBrush(Color.FromRgb(0xC4, 0x65, 0x1E)),
                ComboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0xFF, 0xF5, 0xE6)),
                ComboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0xEF, 0xE5, 0xD6)),
                ComboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E))
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                WindowBackground = new SolidColorBrush(Color.FromRgb(0xFA, 0xF5, 0xF0)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                ButtonBackground = new SolidColorBrush(Color.FromRgb(0xE6, 0x85, 0x3E)),
                ButtonHoverBackground = new SolidColorBrush(Color.FromRgb(0xF6, 0x95, 0x4E)),
                ButtonPressedBackground = new SolidColorBrush(Color.FromRgb(0xD6, 0x75, 0x2E)),
                ButtonDisabledBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                TitleForeground = new SolidColorBrush(Colors.White),
                ButtonForeground = new SolidColorBrush(Colors.White),
                ButtonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
                ButtonOutline = new SolidColorBrush(Color.FromRgb(0xDE, 0xC4, 0xB0))
            }
        };
    }

    /// <summary>
    /// Applies the current theme to the application
    /// </summary>
    public void ApplyTheme()
    {
        if (_currentTheme == null) return;

        // Apply to application resources
        Application.Current.Resources.Clear();

        // Apply window theme
        var windowTheme = _currentTheme.WindowTheme;
        if (windowTheme != null)
        {
            Application.Current.Resources["MainBackground"] = windowTheme.MainBackground;
            Application.Current.Resources["TitleBarBackground"] = windowTheme.TitleBarBackground;
            Application.Current.Resources["MenuBackground"] = windowTheme.MenuBackground;
            Application.Current.Resources["MenuForeground"] = windowTheme.MenuForeground;
            Application.Current.Resources["SplitterBackground"] = windowTheme.SplitterBackground;
            Application.Current.Resources["GroupBoxBorder"] = windowTheme.GroupBoxBorder;
            Application.Current.Resources["ButtonBackground"] = windowTheme.ButtonBackground;
            Application.Current.Resources["ButtonHoverBackground"] = windowTheme.ButtonHoverBackground;
            Application.Current.Resources["ButtonPressedBackground"] = windowTheme.ButtonPressedBackground;
            Application.Current.Resources["ButtonForeground"] = windowTheme.ButtonForeground;
            Application.Current.Resources["PanelBackground"] = windowTheme.PanelBackground;
            Application.Current.Resources["ControlBackground"] = windowTheme.ControlBackground;
            Application.Current.Resources["ControlBorderBrush"] = windowTheme.ControlBorderBrush;
            Application.Current.Resources["ControlHoverBackground"] = windowTheme.ControlHoverBackground;
            Application.Current.Resources["TextForeground"] = windowTheme.TextForeground;
            Application.Current.Resources["LabelForeground"] = windowTheme.LabelForeground;
            Application.Current.Resources["ComboBoxItemBackground"] = windowTheme.ComboBoxItemBackground;
            Application.Current.Resources["ComboBoxItemHoverBackground"] = windowTheme.ComboBoxItemHoverBackground;
            Application.Current.Resources["ComboBoxItemSelectedBackground"] = windowTheme.ComboBoxItemSelectedBackground;
        }

        // Apply to message box style generator
        var messageBoxTheme = _currentTheme.MessageBoxTheme;
        if (messageBoxTheme != null)
        {
            var generator = new MessageBoxStyleGenerator
            {
                TitleBackground = messageBoxTheme.TitleBackground,
                WindowBackground = messageBoxTheme.WindowBackground,
                BorderBrush = messageBoxTheme.BorderBrush,
                ButtonBackground = messageBoxTheme.ButtonBackground,
                ButtonHoverBackground = messageBoxTheme.ButtonHoverBackground,
                ButtonPressedBackground = messageBoxTheme.ButtonPressedBackground,
                ButtonDisabledBackground = messageBoxTheme.ButtonDisabledBackground,
                TitleForeground = messageBoxTheme.TitleForeground,
                ButtonForeground = messageBoxTheme.ButtonForeground,
                ButtonDisabledForeground = messageBoxTheme.ButtonDisabledForeground,
                ButtonOutline = messageBoxTheme.ButtonOutline
            };

            MessageBoxStyleGenerator.SetCurrent(generator);
        }

        // Re-add the styles that were in App.xaml
        AddDefaultStyles();
    }

    private void AddDefaultStyles()
    {
        // This method adds back the styles that were in App.xaml 
        // since we called Clear() on the resources dictionary

        // Button style
        var buttonStyle = new Style(typeof(Button));
        buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, Application.Current.Resources["ButtonBackground"]));
        buttonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Application.Current.Resources["ButtonForeground"]));
        buttonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Application.Current.Resources["ControlBorderBrush"]));
        buttonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 5, 10, 5)));

        var buttonTemplate = new ControlTemplate(typeof(Button));
        var buttonFactory = new FrameworkElementFactory(typeof(Border));
        buttonFactory.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
        buttonFactory.SetBinding(Border.BorderBrushProperty, new System.Windows.Data.Binding("BorderBrush") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
        buttonFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        buttonFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));

        var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        contentPresenterFactory.SetBinding(ContentPresenter.MarginProperty, new System.Windows.Data.Binding("Padding") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });

        buttonFactory.AppendChild(contentPresenterFactory);
        buttonTemplate.VisualTree = buttonFactory;

        var mouseOverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
        mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, Application.Current.Resources["ButtonHoverBackground"]));
        buttonTemplate.Triggers.Add(mouseOverTrigger);

        var pressedTrigger = new Trigger { Property = Button.IsPressedProperty, Value = true };
        pressedTrigger.Setters.Add(new Setter(Button.BackgroundProperty, Application.Current.Resources["ButtonPressedBackground"]));
        buttonTemplate.Triggers.Add(pressedTrigger);

        buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, buttonTemplate));
        Application.Current.Resources["CustomButton"] = buttonStyle;

        // GroupBox style
        var groupBoxStyle = new Style(typeof(GroupBox));
        groupBoxStyle.Setters.Add(new Setter(GroupBox.BorderBrushProperty, Application.Current.Resources["GroupBoxBorder"]));
        groupBoxStyle.Setters.Add(new Setter(GroupBox.BorderThicknessProperty, new Thickness(1)));
        groupBoxStyle.Setters.Add(new Setter(GroupBox.PaddingProperty, new Thickness(10)));
        groupBoxStyle.Setters.Add(new Setter(GroupBox.MarginProperty, new Thickness(5)));

        var groupBoxTemplate = new ControlTemplate(typeof(GroupBox));
        var groupBoxFactory = new FrameworkElementFactory(typeof(Grid));

        var rowDef1 = new FrameworkElementFactory(typeof(RowDefinition));
        rowDef1.SetValue(RowDefinition.HeightProperty, GridLength.Auto);
        groupBoxFactory.AppendChild(rowDef1);

        var rowDef2 = new FrameworkElementFactory(typeof(RowDefinition));
        rowDef2.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));
        groupBoxFactory.AppendChild(rowDef2);

        var borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.SetValue(Grid.RowProperty, 0);
        borderFactory.SetValue(Grid.RowSpanProperty, 2);
        borderFactory.SetBinding(Border.BorderBrushProperty, new System.Windows.Data.Binding("BorderBrush") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
        borderFactory.SetBinding(Border.BorderThicknessProperty, new System.Windows.Data.Binding("BorderThickness") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
        borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
        groupBoxFactory.AppendChild(borderFactory);

        var headerBorderFactory = new FrameworkElementFactory(typeof(Border));
        headerBorderFactory.SetValue(Grid.RowProperty, 0);
        headerBorderFactory.SetValue(Border.BackgroundProperty, Application.Current.Resources["MainBackground"]);
        headerBorderFactory.SetValue(Border.MarginProperty, new Thickness(10, 0, 0, 0));

        var headerContentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
        headerContentFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
        headerContentFactory.SetValue(ContentPresenter.RecognizesAccessKeyProperty, true);
        headerContentFactory.SetValue(ContentPresenter.MarginProperty, new Thickness(5, 0, 5, 0));
        headerBorderFactory.AppendChild(headerContentFactory);
        groupBoxFactory.AppendChild(headerBorderFactory);

        var contentPresenterFactoryGroupBox = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenterFactoryGroupBox.SetValue(Grid.RowProperty, 1);
        contentPresenterFactoryGroupBox.SetBinding(ContentPresenter.MarginProperty, new System.Windows.Data.Binding("Padding") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
        groupBoxFactory.AppendChild(contentPresenterFactoryGroupBox);

        groupBoxTemplate.VisualTree = groupBoxFactory;
        groupBoxStyle.Setters.Add(new Setter(GroupBox.TemplateProperty, groupBoxTemplate));

        var headerTemplate = new DataTemplate();
        var headerTemplateFactory = new FrameworkElementFactory(typeof(TextBlock));
        headerTemplateFactory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding());
        headerTemplateFactory.SetValue(TextBlock.ForegroundProperty, Application.Current.Resources["LabelForeground"]);
        headerTemplateFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
        headerTemplate.VisualTree = headerTemplateFactory;
        groupBoxStyle.Setters.Add(new Setter(GroupBox.HeaderTemplateProperty, headerTemplate));

        Application.Current.Resources["CustomGroupBox"] = groupBoxStyle;

        // ComboBox style
        var comboBoxStyle = new Style(typeof(ComboBox));
        comboBoxStyle.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ControlBackground"]));
        comboBoxStyle.Setters.Add(new Setter(Control.ForegroundProperty, Application.Current.Resources["TextForeground"]));
        comboBoxStyle.Setters.Add(new Setter(Control.BorderBrushProperty, Application.Current.Resources["ControlBorderBrush"]));
        comboBoxStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));

        var comboBoxMouseOver = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        comboBoxMouseOver.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ControlHoverBackground"]));
        comboBoxStyle.Triggers.Add(comboBoxMouseOver);

        var comboBoxFocus = new Trigger { Property = ComboBox.IsKeyboardFocusWithinProperty, Value = true };
        comboBoxFocus.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ControlHoverBackground"]));
        comboBoxStyle.Triggers.Add(comboBoxFocus);

        Application.Current.Resources["CustomComboBox"] = comboBoxStyle;

        // ComboBoxItem style
        var comboBoxItemStyle = new Style(typeof(ComboBoxItem));
        comboBoxItemStyle.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ComboBoxItemBackground"]));
        comboBoxItemStyle.Setters.Add(new Setter(Control.ForegroundProperty, Application.Current.Resources["TextForeground"]));

        var itemMouseOver = new Trigger { Property = ComboBoxItem.IsMouseOverProperty, Value = true };
        itemMouseOver.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ComboBoxItemHoverBackground"]));
        comboBoxItemStyle.Triggers.Add(itemMouseOver);

        var itemSelected = new Trigger { Property = ComboBoxItem.IsSelectedProperty, Value = true };
        itemSelected.Setters.Add(new Setter(Control.BackgroundProperty, Application.Current.Resources["ComboBoxItemSelectedBackground"]));
        comboBoxItemStyle.Triggers.Add(itemSelected);

        Application.Current.Resources["CustomComboBoxItem"] = comboBoxItemStyle;
    }

    /// <summary>
    /// Creates a new theme based on the current theme
    /// </summary>
    public Theme CreateNewThemeBasedOnCurrent(string themeName)
    {
        var newTheme = new Theme
        {
            Name = themeName,
            IsBuiltIn = false,
            IsDarkTheme = _currentTheme.IsDarkTheme,
            WindowTheme = new WindowTheme
            {
                MainBackground = CloneBrush(_currentTheme.WindowTheme.MainBackground),
                TitleBarBackground = CloneBrush(_currentTheme.WindowTheme.TitleBarBackground),
                MenuBackground = CloneBrush(_currentTheme.WindowTheme.MenuBackground),
                MenuForeground = CloneBrush(_currentTheme.WindowTheme.MenuForeground),
                SplitterBackground = CloneBrush(_currentTheme.WindowTheme.SplitterBackground),
                GroupBoxBorder = CloneBrush(_currentTheme.WindowTheme.GroupBoxBorder),
                ButtonBackground = CloneBrush(_currentTheme.WindowTheme.ButtonBackground),
                ButtonHoverBackground = CloneBrush(_currentTheme.WindowTheme.ButtonHoverBackground),
                ButtonPressedBackground = CloneBrush(_currentTheme.WindowTheme.ButtonPressedBackground),
                ButtonForeground = CloneBrush(_currentTheme.WindowTheme.ButtonForeground),
                PanelBackground = CloneBrush(_currentTheme.WindowTheme.PanelBackground),
                ControlBackground = CloneBrush(_currentTheme.WindowTheme.ControlBackground),
                ControlBorderBrush = CloneBrush(_currentTheme.WindowTheme.ControlBorderBrush),
                ControlHoverBackground = CloneBrush(_currentTheme.WindowTheme.ControlHoverBackground),
                TextForeground = CloneBrush(_currentTheme.WindowTheme.TextForeground),
                LabelForeground = CloneBrush(_currentTheme.WindowTheme.LabelForeground),
                ComboBoxItemBackground = CloneBrush(_currentTheme.WindowTheme.ComboBoxItemBackground),
                ComboBoxItemHoverBackground = CloneBrush(_currentTheme.WindowTheme.ComboBoxItemHoverBackground),
                ComboBoxItemSelectedBackground = CloneBrush(_currentTheme.WindowTheme.ComboBoxItemSelectedBackground),
                AlternateTextForeground = CloneBrush(_currentTheme.WindowTheme.AlternateTextForeground),
                MainAccent = CloneBrush(_currentTheme.WindowTheme.MainAccent)
            },
            MessageBoxTheme = new MessageBoxTheme
            {
                TitleBackground = CloneBrush(_currentTheme.MessageBoxTheme.TitleBackground),
                WindowBackground = CloneBrush(_currentTheme.MessageBoxTheme.WindowBackground),
                BorderBrush = CloneBrush(_currentTheme.MessageBoxTheme.BorderBrush),
                ButtonBackground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonBackground),
                ButtonHoverBackground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonHoverBackground),
                ButtonPressedBackground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonPressedBackground),
                ButtonDisabledBackground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonDisabledBackground),
                TitleForeground = CloneBrush(_currentTheme.MessageBoxTheme.TitleForeground),
                ButtonForeground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonForeground),
                ButtonDisabledForeground = CloneBrush(_currentTheme.MessageBoxTheme.ButtonDisabledForeground),
                ButtonOutline = CloneBrush(_currentTheme.MessageBoxTheme.ButtonOutline)
            }
        };

        // Add to custom themes collection
        CustomThemes.Add(newTheme);

        // Save custom themes
        SaveCustomThemes();

        return newTheme;
    }

    /// <summary>
    /// Selects a color using the color picker dialog
    /// </summary>
    /// <param name="initialColor">The initial color to display</param>
    /// <returns>The selected brush, or null if canceled</returns>
    public System.Windows.Media.SolidColorBrush SelectColor(System.Windows.Media.Color initialColor)
    {
        var selectedColor = ShowColorPicker(initialColor);
        if (selectedColor.HasValue)
        {
            return new System.Windows.Media.SolidColorBrush(selectedColor.Value);
        }
        return null;
    }

    private Brush CloneBrush(Brush brush)
    {
        if (brush is SolidColorBrush solidBrush)
        {
            return new SolidColorBrush(solidBrush.Color);
        }
        return brush.Clone();
    }

    /// <summary>
    /// Saves custom themes to a JSON file
    /// </summary>
    public void SaveCustomThemes()
    {
        try
        {
            EnsureThemesDirectoryExists();
            string themesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeForge", "Themes");
            string filePath = Path.Combine(themesFolder, "CustomThemes.json");

            var themeData = new List<ThemeData>();
            foreach (var theme in CustomThemes)
            {
                themeData.Add(theme.ToData());
            }

            string json = JsonSerializer.Serialize(themeData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"Error saving custom themes: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads custom themes from a JSON file
    /// </summary>
    public void LoadCustomThemes()
    {
        try
        {
            EnsureThemesDirectoryExists();
            string themesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeForge", "Themes");

            string filePath = Path.Combine(themesFolder, "CustomThemes.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var themeData = JsonSerializer.Deserialize<List<ThemeData>>(json);

                if (themeData != null)
                {
                    CustomThemes.Clear();
                    foreach (var data in themeData)
                    {
                        CustomThemes.Add(Theme.FromData(data));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"Error loading custom themes: {ex.Message}");
        }
    }

    /// <summary>
    /// Exports a theme to a file
    /// </summary>
    /// <param name="theme">The theme to export</param>
    /// <param name="filePath">The file path to save to</param>
    /// <param name="exportType">What to export (Window, MessageBox, or Both)</param>
    public void ExportTheme(Theme theme, string filePath, ThemeExportType exportType)
    {
        try
        {
            var themeData = theme.ToData(exportType);
            string json = JsonSerializer.Serialize(themeData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"Error exporting theme: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Shows the theme export dialog and exports the theme
    /// </summary>
    /// <param name="theme">The theme to export</param>
    /// <param name="filePath">The path to export to</param>
    /// <returns>True if the export was successful, false otherwise</returns>
    public bool ShowThemeExportDialog(Theme theme, string filePath)
    {
        var dialog = new Dialogs.ThemeExportDialog();
        if (dialog.ShowDialog() == true)
        {
            ExportTheme(theme, filePath, dialog.ExportType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Imports a theme from a file
    /// </summary>
    /// <param name="filePath">The file path to import from</param>
    /// <returns>The imported theme</returns>
    public Theme ImportTheme(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var themeData = JsonSerializer.Deserialize<ThemeData>(json);

            if (themeData != null)
            {
                var theme = Theme.FromData(themeData);

                // Add to custom themes if it doesn't already exist
                if (!CustomThemes.Any(t => t.Name == theme.Name))
                {
                    CustomThemes.Add(theme);
                    SaveCustomThemes();
                }

                return theme;
            }

            throw new Exception("Invalid theme data");
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"Error importing theme: {ex.Message}");
            throw;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}