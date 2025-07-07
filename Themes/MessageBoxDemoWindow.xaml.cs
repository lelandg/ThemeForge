using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace ThemeForge.Themes
{
    /// <summary>
    /// Interaction logic for MessageBoxDemoWindow.xaml
    /// </summary>
    public partial class MessageBoxDemoWindow : Window
    {
        public MessageBoxDemoWindow()
        {
            InitializeComponent();
            
            // Enable window dragging by clicking on the title bar
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };

            // Add escape key handler
            KeyDown += MessageBoxDemoWindow_KeyDown;
        }

        private void MessageBoxDemoWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        #region Title Bar Controls

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Button Type Demos

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This is a message with an OK button.\n\nNotice how it uses the current theme colors for the title bar, buttons, and background.",
                "OK Button Demo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DisplayResult("OK Button", result);
        }

        private void BtnOKCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has OK and Cancel buttons.\nClick either button to see the result.\n\nThe theme colors are automatically applied to both buttons.",
                "OK/Cancel Demo",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            DisplayResult("OK/Cancel Buttons", result);
        }

        private void BtnYesNo_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has Yes and No buttons.\nWould you like to proceed?\n\nBoth buttons inherit the current theme styling.",
                "Yes/No Demo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            DisplayResult("Yes/No Buttons", result);
        }

        private void BtnYesNoCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has Yes, No, and Cancel buttons.\nDo you want to save your changes?\n\nAll three buttons use the current theme colors.",
                "Yes/No/Cancel Demo",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            DisplayResult("Yes/No/Cancel Buttons", result);
        }

        #endregion

        #region Icon Type Demos

        private void BtnInformation_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This is an information message.\nIt uses the Information icon.\n\nThe icon is displayed alongside the themed message box.",
                "Information Icon",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DisplayResult("Information Icon", result);
        }

        private void BtnWarning_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This is a warning message.\nIt uses the Warning icon.\n\nWarning messages help alert users to potential issues.",
                "Warning Icon",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            DisplayResult("Warning Icon", result);
        }

        private void BtnError_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This is an error message.\nIt uses the Error icon.\n\nError messages indicate something went wrong.",
                "Error Icon",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            DisplayResult("Error Icon", result);
        }

        private void BtnQuestion_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This is a question message.\nIt uses the Question icon.\n\nQuestion icons are perfect for user confirmation dialogs.",
                "Question Icon",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            DisplayResult("Question Icon", result);
        }

        private void BtnNoIcon_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This message box has no icon.\nIt uses MessageBoxImage.None.\n\nSometimes a clean look without icons is preferred.",
                "No Icon",
                MessageBoxButton.OK,
                MessageBoxImage.None);

            DisplayResult("No Icon", result);
        }

        #endregion

        #region Default Result Demos

        private void BtnDefaultYes_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has a default result of Yes.\nPressing Enter will select Yes.\n\nNotice the Yes button is highlighted as the default.",
                "Default Yes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Yes);

            DisplayResult("Default Yes", result);
        }

        private void BtnDefaultNo_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has a default result of No.\nPressing Enter will select No.\n\nThe No button should be highlighted as the default.",
                "Default No",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            DisplayResult("Default No", result);
        }

        private void BtnDefaultCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show(
                "This dialog has a default result of Cancel.\nPressing Enter will select Cancel.\n\nThe Cancel button should be highlighted as the default.",
                "Default Cancel",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            DisplayResult("Default Cancel", result);
        }

        #endregion

        #region Theme Integration Demos

        private void BtnCurrentTheme_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeManager.Current.CurrentTheme;
            var result = CustomMessageBox.Show(
                $"This message box is using the current theme: '{currentTheme.Name}'\n\n" +
                "• Title bar uses theme's title background color\n" +
                "• Buttons use theme's button colors\n" +
                "• Text uses theme's foreground colors\n" +
                "• Background uses theme's window background\n\n" +
                "Try switching themes in the main window to see how this dialog adapts!",
                $"Current Theme: {currentTheme.Name}",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DisplayResult("Current Theme Demo", result);
        }

        private void BtnCustomStyle_Click(object sender, RoutedEventArgs e)
        {
            // Create a custom style that contrasts with the current theme
            var customStyle = new MessageBoxStyleGenerator();
            
            // Use a purple theme for demonstration
            customStyle.TitleBackground = new SolidColorBrush(Colors.Purple);
            customStyle.BorderBrush = new SolidColorBrush(Colors.Purple);
            customStyle.ButtonBackground = new SolidColorBrush(Colors.Purple);
            customStyle.ButtonHoverBackground = new SolidColorBrush(Colors.MediumPurple);
            customStyle.ButtonPressedBackground = new SolidColorBrush(Colors.DarkMagenta);
            customStyle.WindowBackground = new SolidColorBrush(Color.FromRgb(250, 245, 255));
            customStyle.TitleForeground = new SolidColorBrush(Colors.White);
            customStyle.ButtonForeground = new SolidColorBrush(Colors.White);

            // Apply the custom style temporarily
            var originalStyle = MessageBoxStyleGenerator.Current;
            MessageBoxStyleGenerator.SetCurrent(customStyle);

            var result = CustomMessageBox.Show(
                "This message box uses a custom purple style that overrides the current theme.\n\n" +
                "This demonstrates how you can create one-off styled dialogs while maintaining the overall theme for other dialogs.",
                "Custom Style Demo",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            // Restore the original style
            MessageBoxStyleGenerator.SetCurrent(originalStyle);

            DisplayResult("Custom Style Demo", result);
        }

        private void BtnSequenceDemo_Click(object sender, RoutedEventArgs e)
        {
            // Show a sequence of different dialogs to demonstrate consistency
            var result1 = CustomMessageBox.Show(
                "This is the first dialog in a sequence.\nAll dialogs will use consistent theming.",
                "Sequence Demo - Step 1",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var result2 = CustomMessageBox.Show(
                "This is the second dialog.\nNotice the consistent styling with the first dialog.",
                "Sequence Demo - Step 2",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            string finalMessage = "Sequence completed!\n\n";
            finalMessage += $"Step 1 result: {result1}\n";
            finalMessage += $"Step 2 result: {result2}\n\n";
            finalMessage += "All dialogs maintained consistent theming throughout the sequence.";

            var result3 = CustomMessageBox.Show(
                finalMessage,
                "Sequence Demo - Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DisplayResult($"Sequence Demo: {result1}, {result2}, {result3}", result3);
        }

        #endregion

        private void DisplayResult(string dialogType, MessageBoxResult result)
        {
            txtResult.Text = $"Dialog: {dialogType}\nResult: {result}\nTime: {DateTime.Now:HH:mm:ss}";
        }
    }
}