using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using System.ComponentModel;

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
                    ComboBoxBackground = ((SolidColorBrush)WindowTheme.ComboBoxBackground).Color.ToString(),
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
                    ComboBoxBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.WindowTheme.ComboBoxBackground)),
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
    public class WindowTheme : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Brush _mainBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA));
        public Brush MainBackground { get => _mainBackground; set { if (_mainBackground != value) { _mainBackground = value; OnPropertyChanged(nameof(MainBackground)); } } }

        private Brush _titleBarBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush TitleBarBackground { get => _titleBarBackground; set { if (_titleBarBackground != value) { _titleBarBackground = value; OnPropertyChanged(nameof(TitleBarBackground)); } } }

        private Brush _mainAccent = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush MainAccent { get => _mainAccent; set { if (_mainAccent != value) { _mainAccent = value; OnPropertyChanged(nameof(MainAccent)); } } }

        private Brush _menuBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush MenuBackground { get => _menuBackground; set { if (_menuBackground != value) { _menuBackground = value; OnPropertyChanged(nameof(MenuBackground)); } } }

        private Brush _menuForeground = new SolidColorBrush(Colors.White);
        public Brush MenuForeground { get => _menuForeground; set { if (_menuForeground != value) { _menuForeground = value; OnPropertyChanged(nameof(MenuForeground)); } } }

        private Brush _alternateTextForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
        public Brush AlternateTextForeground { get => _alternateTextForeground; set { if (_alternateTextForeground != value) { _alternateTextForeground = value; OnPropertyChanged(nameof(AlternateTextForeground)); } } }

        private Brush _splitterBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush SplitterBackground { get => _splitterBackground; set { if (_splitterBackground != value) { _splitterBackground = value; OnPropertyChanged(nameof(SplitterBackground)); } } }

        private Brush _groupBoxBorder = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));
        public Brush GroupBoxBorder { get => _groupBoxBorder; set { if (_groupBoxBorder != value) { _groupBoxBorder = value; OnPropertyChanged(nameof(GroupBoxBorder)); } } }

        private Brush _buttonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush ButtonBackground { get => _buttonBackground; set { if (_buttonBackground != value) { _buttonBackground = value; OnPropertyChanged(nameof(ButtonBackground)); } } }

        private Brush _buttonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6));
        public Brush ButtonHoverBackground { get => _buttonHoverBackground; set { if (_buttonHoverBackground != value) { _buttonHoverBackground = value; OnPropertyChanged(nameof(ButtonHoverBackground)); } } }

        private Brush _buttonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6));
        public Brush ButtonPressedBackground { get => _buttonPressedBackground; set { if (_buttonPressedBackground != value) { _buttonPressedBackground = value; OnPropertyChanged(nameof(ButtonPressedBackground)); } } }

        private Brush _buttonForeground = new SolidColorBrush(Colors.White);
        public Brush ButtonForeground { get => _buttonForeground; set { if (_buttonForeground != value) { _buttonForeground = value; OnPropertyChanged(nameof(ButtonForeground)); } } }

        private Brush _panelBackground = new SolidColorBrush(Colors.Transparent);
        public Brush PanelBackground { get => _panelBackground; set { if (_panelBackground != value) { _panelBackground = value; OnPropertyChanged(nameof(PanelBackground)); } } }

        private Brush _controlBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF));
        public Brush ControlBackground { get => _controlBackground; set { if (_controlBackground != value) { _controlBackground = value; OnPropertyChanged(nameof(ControlBackground)); } } }

        private Brush _comboBoxBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF));
        public Brush ComboBoxBackground { get => _comboBoxBackground; set { if (_comboBoxBackground != value) { _comboBoxBackground = value; OnPropertyChanged(nameof(ComboBoxBackground)); } } }

        private Brush _controlBorderBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));
        public Brush ControlBorderBrush { get => _controlBorderBrush; set { if (_controlBorderBrush != value) { _controlBorderBrush = value; OnPropertyChanged(nameof(ControlBorderBrush)); } } }

        private Brush _controlHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF));
        public Brush ControlHoverBackground { get => _controlHoverBackground; set { if (_controlHoverBackground != value) { _controlHoverBackground = value; OnPropertyChanged(nameof(ControlHoverBackground)); } } }

        private Brush _textForeground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
        public Brush TextForeground { get => _textForeground; set { if (_textForeground != value) { _textForeground = value; OnPropertyChanged(nameof(TextForeground)); } } }

        private Brush _labelForeground = new SolidColorBrush(Color.FromRgb(0x1E, 0x5A, 0x9C));
        public Brush LabelForeground { get => _labelForeground; set { if (_labelForeground != value) { _labelForeground = value; OnPropertyChanged(nameof(LabelForeground)); } } }

        private Brush _comboBoxItemBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF));
        public Brush ComboBoxItemBackground { get => _comboBoxItemBackground; set { if (_comboBoxItemBackground != value) { _comboBoxItemBackground = value; OnPropertyChanged(nameof(ComboBoxItemBackground)); } } }

        private Brush _comboBoxItemHoverBackground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE5, 0xEF));
        public Brush ComboBoxItemHoverBackground { get => _comboBoxItemHoverBackground; set { if (_comboBoxItemHoverBackground != value) { _comboBoxItemHoverBackground = value; OnPropertyChanged(nameof(ComboBoxItemHoverBackground)); } } }

        private Brush _comboBoxItemSelectedBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush ComboBoxItemSelectedBackground { get => _comboBoxItemSelectedBackground; set { if (_comboBoxItemSelectedBackground != value) { _comboBoxItemSelectedBackground = value; OnPropertyChanged(nameof(ComboBoxItemSelectedBackground)); } } }
    }

    /// <summary>
    /// Contains theme settings for message boxes
    /// </summary>
    public class MessageBoxTheme : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Brush _titleBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush TitleBackground { get => _titleBackground; set { if (_titleBackground != value) { _titleBackground = value; OnPropertyChanged(nameof(TitleBackground)); } } }

        private Brush _windowBackground = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA));
        public Brush WindowBackground { get => _windowBackground; set { if (_windowBackground != value) { _windowBackground = value; OnPropertyChanged(nameof(WindowBackground)); } } }

        private Brush _borderBrush = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush BorderBrush { get => _borderBrush; set { if (_borderBrush != value) { _borderBrush = value; OnPropertyChanged(nameof(BorderBrush)); } } }

        private Brush _buttonBackground = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush ButtonBackground { get => _buttonBackground; set { if (_buttonBackground != value) { _buttonBackground = value; OnPropertyChanged(nameof(ButtonBackground)); } } }

        private Brush _buttonHoverBackground = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6));
        public Brush ButtonHoverBackground { get => _buttonHoverBackground; set { if (_buttonHoverBackground != value) { _buttonHoverBackground = value; OnPropertyChanged(nameof(ButtonHoverBackground)); } } }

        private Brush _buttonPressedBackground = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6));
        public Brush ButtonPressedBackground { get => _buttonPressedBackground; set { if (_buttonPressedBackground != value) { _buttonPressedBackground = value; OnPropertyChanged(nameof(ButtonPressedBackground)); } } }

        private Brush _buttonDisabledBackground = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
        public Brush ButtonDisabledBackground { get => _buttonDisabledBackground; set { if (_buttonDisabledBackground != value) { _buttonDisabledBackground = value; OnPropertyChanged(nameof(ButtonDisabledBackground)); } } }

        private Brush _titleForeground = new SolidColorBrush(Colors.White);
        public Brush TitleForeground { get => _titleForeground; set { if (_titleForeground != value) { _titleForeground = value; OnPropertyChanged(nameof(TitleForeground)); } } }

        private Brush _buttonForeground = new SolidColorBrush(Colors.White);
        public Brush ButtonForeground { get => _buttonForeground; set { if (_buttonForeground != value) { _buttonForeground = value; OnPropertyChanged(nameof(ButtonForeground)); } } }

        private Brush _buttonDisabledForeground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        public Brush ButtonDisabledForeground { get => _buttonDisabledForeground; set { if (_buttonDisabledForeground != value) { _buttonDisabledForeground = value; OnPropertyChanged(nameof(ButtonDisabledForeground)); } } }

        private Brush _buttonOutline = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));
        public Brush ButtonOutline { get => _buttonOutline; set { if (_buttonOutline != value) { _buttonOutline = value; OnPropertyChanged(nameof(ButtonOutline)); } } }
    }
}
