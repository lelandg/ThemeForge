using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace ThemeForge.Themes
{
    public partial class CustomMessageBox : Window
    {
        private MessageBoxResult _result = MessageBoxResult.None;

        private CustomMessageBox()
        {
            InitializeComponent();
            ApplyMessageBoxStyle();
        }

        private void ApplyMessageBoxStyle()
        {
            var generator = MessageBoxStyleGenerator.Current;

            // Apply colors from generator
            Background = generator.WindowBackground;
            MainBorder.BorderBrush = generator.BorderBrush;
            MessageTitle.Background = generator.TitleBackground;
            MessageTitle.Foreground = generator.TitleForeground;

            Resources["MessageBoxButtonBackground"] = generator.ButtonBackground;
            Resources["MessageBoxButtonHoverBackground"] = generator.ButtonHoverBackground;
            Resources["MessageBoxButtonPressedBackground"] = generator.ButtonPressedBackground;
            Resources["MessageBoxButtonDisabledBackground"] = generator.ButtonDisabledBackground;
            Resources["MessageBoxButtonForeground"] = generator.ButtonForeground;
            Resources["MessageBoxButtonDisabledForeground"] = generator.ButtonDisabledForeground;
            Resources["MessageBoxButtonOutline"] = generator.ButtonOutline;
        }

        /// <summary>
        /// Shows a message box with the specified message, caption, and buttons
        /// </summary>
        public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            var messageBox = new CustomMessageBox
            {
                Title = caption,
                MessageTitle = { Text = caption },
                MessageText = { Text = messageText },
                Owner = Application.Current.MainWindow
            };

            // Set up message icon
            messageBox.SetMessageIcon(icon);

            // Configure buttons based on MessageBoxButton enum
            messageBox.ConfigureButtons(button);

            // Show the dialog
            messageBox.ShowDialog();

            return messageBox._result;
        }

        /// <summary>
        /// Configures the buttons based on the MessageBoxButton enum
        /// </summary>
        private void ConfigureButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.OKCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// Sets the message icon based on the MessageBoxImage enum
        /// </summary>
        private void SetMessageIcon(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Information:
                    // Use a standard information icon
                    MessageIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ThemeForge;component/Resources/Info.png", UriKind.Absolute));
                    break;

                case MessageBoxImage.Warning:
                    // Use a standard warning icon
                    MessageIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ThemeForge;component/Resources/Warning.png", UriKind.Absolute));
                    break;

                case MessageBoxImage.Error:
                    // Use a standard error icon
                    MessageIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ThemeForge;component/Resources/Error.png", UriKind.Absolute));
                    break;

                case MessageBoxImage.Question:
                    // Use a standard question icon
                    MessageIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ThemeForge;component/Resources/Question.png", UriKind.Absolute));
                    break;

                default:
                    // No icon
                    MessageIcon.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            Close();
        }
    }
}
