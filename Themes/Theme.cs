using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace ThemeForge.Themes
{
    /// <summary>
    /// Represents a complete UI theme with settings for both windows and message boxes
    /// </summary>
    public class Theme
    {
        /// <summary>The theme name</summary>
        public string Name { get; set; } = "Default";

        /// <summary>Whether this is a built-in theme</summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>Whether this is a dark theme</summary>
        public bool IsDarkTheme { get; set; }

        /// <summary>The window theme settings</summary>
        public WindowTheme WindowTheme { get; set; } = new WindowTheme();

        /// <summary>The message box theme settings</summary>
        public MessageBoxTheme MessageBoxTheme { get; set; } = new MessageBoxTheme();

        /// <summary>
        /// Converts the theme to a serializable data object
        /// </summary>
        /// <param name="exportType">What parts of the theme to include</param>
        public ThemeData ToData(ThemeExportType exportType = ThemeExportType.Both)
        {
            var data = new ThemeData
            {
                Name = Name,
                IsDarkTheme = IsDarkTheme
            };

            // Include window theme if requested
            if (exportType == ThemeExportType.Window || exportType == ThemeExportType.Both)
            {
                data.WindowTheme = new WindowThemeData
                {
                    MainBackground = ((SolidColorBrush)WindowTheme.MainBackground).Color.ToString(),
                    TitleBarBackground = ((SolidColorBrush)WindowTheme.TitleBarBackground).Color.ToString(),
                    MainAccent = ((SolidColorBrush)WindowTheme.MainAccent).Color.ToString(),
                    MenuBackground = ((SolidColorBrush)WindowTheme.MenuBackground).Color.ToString(),
                    MenuForeground = ((SolidColorBrush)WindowTheme.MenuForeground).Color.ToString(),
                    AlternateTextForeground = ((SolidColorBrush)WindowTheme.AlternateTextForeground).Color.ToString(),
                    SplitterBackground = ((SolidColorBrush)WindowTheme.SplitterBackground).Color.ToString(),
                    GroupBoxBorder = ((SolidColorBrush)WindowTheme.GroupBoxBorder).Color.ToString(),
                    ButtonBackground = ((SolidColorBrush)WindowTheme.ButtonBackground).Color.ToString(),
                    ButtonHoverBackground = ((SolidColorBrush)WindowTheme.ButtonHoverBackground).Color.ToString(),
                    ButtonPressedBackground = ((SolidColorBrush)WindowTheme.ButtonPressedBackground).Color.ToString(),
                    ButtonForeground = ((SolidColorBrush)WindowTheme.ButtonForeground).Color.ToString(),
                    PanelBackground = ((SolidColorBrush)WindowTheme.PanelBackground).Color.ToString(),
                    ControlBackground = ((SolidColorBrush)WindowTheme.ControlBackground).Color.ToString(),
                    ControlBorderBrush = ((SolidColorBrush)WindowTheme.ControlBorderBrush).Color.ToString(),
                    ControlHoverBackground = ((SolidColorBrush)WindowTheme.ControlHoverBackground).Color.ToString(),
                    TextForeground = ((SolidColorBrush)WindowTheme.TextForeground).Color.ToString(),
                    LabelForeground = ((SolidColorBrush)WindowTheme.LabelForeground).Color.ToString(),
                    ComboBoxItemBackground = ((SolidColorBrush)WindowTheme.ComboBoxItemBackground).Color.ToString(),
                    ComboBoxItemHoverBackground = ((SolidColorBrush)WindowTheme.ComboBoxItemHoverBackground).Color.ToString(),
                    ComboBoxItemSelectedBackground = ((SolidColorBrush)WindowTheme.ComboBoxItemSelectedBackground).Color.ToString()
                };
            }

            // Include message box theme if requested
            if (exportType == ThemeExportType.MessageBox || exportType == ThemeExportType.Both)
            {
                data.MessageBoxTheme = new MessageBoxThemeData
                {
                    TitleBackground = ((SolidColorBrush)MessageBoxTheme.TitleBackground).Color.ToString(),
                    WindowBackground = ((SolidColorBrush)MessageBoxTheme.WindowBackground).Color.ToString(),
                    BorderBrush = ((SolidColorBrush)MessageBoxTheme.BorderBrush).Color.ToString(),
                    ButtonBackground = ((SolidColorBrush)MessageBoxTheme.ButtonBackground).Color.ToString(),
                    ButtonHoverBackground = ((SolidColorBrush)MessageBoxTheme.ButtonHoverBackground).Color.ToString(),
                    ButtonPressedBackground = ((SolidColorBrush)MessageBoxTheme.ButtonPressedBackground).Color.ToString(),
                    ButtonDisabledBackground = ((SolidColorBrush)MessageBoxTheme.ButtonDisabledBackground).Color.ToString(),
                    TitleForeground = ((SolidColorBrush)MessageBoxTheme.TitleForeground).Color.ToString(),
                    ButtonForeground = ((SolidColorBrush)MessageBoxTheme.ButtonForeground).Color.ToString(),
                    ButtonDisabledForeground = ((SolidColorBrush)MessageBoxTheme.ButtonDisabledForeground).Color.ToString(),
                    ButtonOutline = ((SolidColorBrush)MessageBoxTheme.ButtonOutline).Color.ToString()
                };
            }

            return data;
        }

        /// <summary>
        /// Creates a theme from serialized data
        /// </summary>
        public static Theme FromData(ThemeData data)
        {
            var theme = new Theme
            {
                Name = data.Name,
                IsBuiltIn = false,
                IsDarkTheme = data.IsDarkTheme
            };

            // Load window theme if provided
            if (data.WindowTheme != null)
            {
                theme.WindowTheme = new WindowTheme
                {
                    MainBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.MainBackground)),
                    TitleBarBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.TitleBarBackground)),
                    MainAccent = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.MainAccent)),
                    MenuBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.MenuBackground)),
                    MenuForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.MenuForeground)),
                    AlternateTextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.AlternateTextForeground)),
                    SplitterBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.SplitterBackground)),
                    GroupBoxBorder = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.GroupBoxBorder)),
                    ButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ButtonBackground)),
                    ButtonHoverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ButtonHoverBackground)),
                    ButtonPressedBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ButtonPressedBackground)),
                    ButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ButtonForeground)),
                    PanelBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.PanelBackground)),
                    ControlBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ControlBackground)),
                    ControlBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ControlBorderBrush)),
                    ControlHoverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ControlHoverBackground)),
                    TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.TextForeground)),
                    LabelForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.LabelForeground)),
                    ComboBoxItemBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ComboBoxItemBackground)),
                    ComboBoxItemHoverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ComboBoxItemHoverBackground)),
                    ComboBoxItemSelectedBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ComboBoxItemSelectedBackground))
                };
            }

            // Load message box theme if provided
            if (data.MessageBoxTheme != null)
            {
                theme.MessageBoxTheme = new MessageBoxTheme
                {
                    TitleBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.TitleBackground)),
                    WindowBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.WindowBackground)),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.BorderBrush)),
                    ButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonBackground)),
                    ButtonHoverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonHoverBackground)),
                    ButtonPressedBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonPressedBackground)),
                    ButtonDisabledBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonDisabledBackground)),
                    TitleForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.TitleForeground)),
                    ButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonForeground)),
                    ButtonDisabledForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonDisabledForeground)),
                    ButtonOutline = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.MessageBoxTheme.ButtonOutline))
                };
            }

            return theme;
        }
    }

    /// <summary>
    /// Contains theme settings for windows
    /// </summary>
    public class WindowTheme
    {
        /// <summary>Background color for the main window</summary>
        public Brush MainBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA));

        /// <summary>Background color for title bars</summary>
        public Brush TitleBarBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Accent color for the main elements</summary>
        public Brush MainAccent { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Background color for menus</summary>
        public Brush MenuBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Foreground color for menu text</summary>
        public Brush MenuForeground { get; set; } = new SolidColorBrush(Colors.White);

        /// <summary>Foreground color for alternate text</summary>
        public Brush AlternateTextForeground { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        /// <summary>Background color for splitters</summary>
        public Brush SplitterBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Border color for group boxes</summary>
        public Brush GroupBoxBorder { get; set; } = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));

        /// <summary>Background color for buttons</summary>
        public Brush ButtonBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Background color for buttons when hovered</summary>
        public Brush ButtonHoverBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6));

        /// <summary>Background color for buttons when pressed</summary>
        public Brush ButtonPressedBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6));

        /// <summary>Foreground color for buttons</summary>
        public Brush ButtonForeground { get; set; } = new SolidColorBrush(Colors.White);

        /// <summary>Background color for panels</summary>
        public Brush PanelBackground { get; set; } = new SolidColorBrush(Colors.Transparent);

        /// <summary>Background color for controls</summary>
        public Brush ControlBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF));

        /// <summary>Border color for controls</summary>
        public Brush ControlBorderBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));

        /// <summary>Background color for controls when hovered</summary>
        public Brush ControlHoverBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF));

        /// <summary>Foreground color for text</summary>
        public Brush TextForeground { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        /// <summary>Foreground color for labels</summary>
        public Brush LabelForeground { get; set; } = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C));

        /// <summary>Background color for combo box items</summary>
        public Brush ComboBoxItemBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF));

        /// <summary>Background color for combo box items when hovered</summary>
        public Brush ComboBoxItemHoverBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF));

        /// <summary>Background color for combo box items when selected</summary>
        public Brush ComboBoxItemSelectedBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
    }

    /// <summary>
    /// Contains theme settings for message boxes
    /// </summary>
    public class MessageBoxTheme
    {
        /// <summary>Background color for the message box title</summary>
        public Brush TitleBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Background color for the message box window</summary>
        public Brush WindowBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA));

        /// <summary>Border color for the message box</summary>
        public Brush BorderBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Background color for message box buttons</summary>
        public Brush ButtonBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));

        /// <summary>Background color for message box buttons when hovered</summary>
        public Brush ButtonHoverBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6));

        /// <summary>Background color for message box buttons when pressed</summary>
        public Brush ButtonPressedBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6));

        /// <summary>Background color for message box buttons when disabled</summary>
        public Brush ButtonDisabledBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));

        /// <summary>Foreground color for the message box title</summary>
        public Brush TitleForeground { get; set; } = new SolidColorBrush(Colors.White);

        /// <summary>Foreground color for message box buttons</summary>
        public Brush ButtonForeground { get; set; } = new SolidColorBrush(Colors.White);

        /// <summary>Foreground color for message box buttons when disabled</summary>
        public Brush ButtonDisabledForeground { get; set; } = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));

        /// <summary>Outline color for message box buttons</summary>
        public Brush ButtonOutline { get; set; } = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));
    }
}
