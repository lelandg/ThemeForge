namespace ThemeForge.Themes
{
    /// <summary>
    /// Serializable data structure for themes
    /// </summary>
    public class ThemeData
    {
        /// <summary>The theme name</summary>
        public string Name { get; set; } = "Default";

        /// <summary>Whether this is a dark theme</summary>
        public bool IsDarkTheme { get; set; }

        /// <summary>The window theme settings</summary>
        public WindowThemeData? WindowTheme { get; set; }

        /// <summary>The message box theme settings</summary>
        public MessageBoxThemeData? MessageBoxTheme { get; set; }
    }

    /// <summary>
    /// Serializable data for window themes
    /// </summary>
    public class WindowThemeData
    {
        /// <summary>Background color for the main window</summary>
        public string MainBackground { get; set; } = "#F0F5FA";

        /// <summary>Background color for title bars</summary>
        public string TitleBarBackground { get; set; } = "#3E85C6";

        /// <summary>Accent color for the main elements</summary>
        public string MainAccent { get; set; } = "#3E85C6";

        /// <summary>Background color for menus</summary>
        public string MenuBackground { get; set; } = "#3E85C6";

        /// <summary>Foreground color for menu text</summary>
        public string MenuForeground { get; set; } = "#FFFFFF";

        /// <summary>Foreground color for alternate text</summary>
        public string AlternateTextForeground { get; set; } = "#333333";

        /// <summary>Background color for splitters</summary>
        public string SplitterBackground { get; set; } = "#3E85C6";

        /// <summary>Border color for group boxes</summary>
        public string GroupBoxBorder { get; set; } = "#B0C4DE";

        /// <summary>Background color for buttons</summary>
        public string ButtonBackground { get; set; } = "#3E85C6";

        /// <summary>Background color for buttons when hovered</summary>
        public string ButtonHoverBackground { get; set; } = "#5E95D6";

        /// <summary>Background color for buttons when pressed</summary>
        public string ButtonPressedBackground { get; set; } = "#2E75B6";

        /// <summary>Foreground color for buttons</summary>
        public string ButtonForeground { get; set; } = "#FFFFFF";

        /// <summary>Background color for panels</summary>
        public string PanelBackground { get; set; } = "Transparent";

        /// <summary>Background color for controls</summary>
        public string ControlBackground { get; set; } = "#F0F5FF";

        /// <summary>Background color for combo boxes</summary>
        public string ComboBoxBackground { get; set; } = "#F0F5FF";

        /// <summary>Border color for controls</summary>
        public string ControlBorderBrush { get; set; } = "#B0C4DE";

        /// <summary>Background color for controls when hovered</summary>
        public string ControlHoverBackground { get; set; } = "#E0E5EF";

        /// <summary>Foreground color for text</summary>
        public string TextForeground { get; set; } = "#333333";

        /// <summary>Foreground color for labels</summary>
        public string LabelForeground { get; set; } = "#1E5A9C";

        /// <summary>Background color for combo box items</summary>
        public string ComboBoxItemBackground { get; set; } = "#F0F5FF";

        /// <summary>Background color for combo box items when hovered</summary>
        public string ComboBoxItemHoverBackground { get; set; } = "#E0E5EF";

        /// <summary>Background color for combo box items when selected</summary>
        public string ComboBoxItemSelectedBackground { get; set; } = "#3E85C6";
    }

    /// <summary>
    /// Serializable data for message box themes
    /// </summary>
    public class MessageBoxThemeData
    {
        /// <summary>Background color for the message box title</summary>
        public string TitleBackground { get; set; } = "#3E85C6";

        /// <summary>Background color for the message box window</summary>
        public string WindowBackground { get; set; } = "#F0F5FA";

        /// <summary>Border color for the message box</summary>
        public string BorderBrush { get; set; } = "#3E85C6";

        /// <summary>Background color for message box buttons</summary>
        public string ButtonBackground { get; set; } = "#3E85C6";

        /// <summary>Background color for message box buttons when hovered</summary>
        public string ButtonHoverBackground { get; set; } = "#5E95D6";

        /// <summary>Background color for message box buttons when pressed</summary>
        public string ButtonPressedBackground { get; set; } = "#2E75B6";

        /// <summary>Background color for message box buttons when disabled</summary>
        public string ButtonDisabledBackground { get; set; } = "#DDDDDD";

        /// <summary>Foreground color for the message box title</summary>
        public string TitleForeground { get; set; } = "#FFFFFF";

        /// <summary>Foreground color for message box buttons</summary>
        public string ButtonForeground { get; set; } = "#FFFFFF";

        /// <summary>Foreground color for message box buttons when disabled</summary>
        public string ButtonDisabledForeground { get; set; } = "#999999";

        /// <summary>Outline color for message box buttons</summary>
        public string ButtonOutline { get; set; } = "#B0C4DE";
    }

    /// <summary>
    /// Defines what parts of a theme to export
    /// </summary>
    public enum ThemeExportType
    {
        /// <summary>Export only window theme</summary>
        Window,

        /// <summary>Export only message box theme</summary>
        MessageBox,

        /// <summary>Export both window and message box themes</summary>
        Both
    }
}
