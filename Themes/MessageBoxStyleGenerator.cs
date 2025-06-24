using System;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using SystemColors = System.Windows.SystemColors;

namespace ThemeForge.Themes
{
    /// <summary>
    /// Generator for customizing message box appearance across the application
    /// </summary>
    public class MessageBoxStyleGenerator
    {
        private static MessageBoxStyleGenerator? _instance;

        // Color properties
        public Brush TitleBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush WindowBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFA));
        public Brush BorderBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush ButtonBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x3E, 0x85, 0xC6));
        public Brush ButtonHoverBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x5E, 0x95, 0xD6));
        public Brush ButtonPressedBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0x2E, 0x75, 0xB6));
        public Brush ButtonDisabledBackground { get; set; } = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
        public Brush TitleForeground { get; set; } = Brushes.White;
        public Brush ButtonForeground { get; set; } = Brushes.White;
        public Brush ButtonDisabledForeground { get; set; } = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        public Brush ButtonOutline { get; set; } = new SolidColorBrush(Color.FromRgb(0xB0, 0xC4, 0xDE));

        public static MessageBoxStyleGenerator Current => _instance ??= new MessageBoxStyleGenerator();

        /// <summary>
        /// Sets a new generator instance as the current one
        /// </summary>
        /// <param name="generator">The generator to set as current</param>
        public static void SetCurrent(MessageBoxStyleGenerator generator)
        {
            _instance = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        /// <summary>
        /// Creates a new style generator that uses the system dialog colors
        /// </summary>
        /// <returns>A new style generator with system colors</returns>
        public static MessageBoxStyleGenerator CreateSystemColorsGenerator()
        {
            var generator = new MessageBoxStyleGenerator
            {
                // Use SystemColors brushes for dialog appearance
                TitleBackground = SystemColors.ActiveCaptionBrush,
                WindowBackground = SystemColors.ControlBrush,
                BorderBrush = SystemColors.ActiveBorderBrush,
                ButtonBackground = SystemColors.ControlBrush,
                ButtonHoverBackground = SystemColors.ControlLightBrush,
                ButtonPressedBackground = SystemColors.ControlDarkBrush,
                ButtonDisabledBackground = SystemColors.ControlLightLightBrush,
                TitleForeground = SystemColors.ActiveCaptionTextBrush,
                ButtonForeground = SystemColors.ControlTextBrush,
                ButtonDisabledForeground = SystemColors.GrayTextBrush,
                ButtonOutline = SystemColors.ActiveBorderBrush
            };

            return generator;
        }

        /// <summary>
        /// Resets the current generator to null, causing a new default one to be created on next access
        /// </summary>
        public static void ResetToDefaults()
        {
            _instance = null;
        }
    }
}
