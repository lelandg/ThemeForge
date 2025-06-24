using System.Windows;

namespace ThemeForge.Themes
{
    public partial class TextInputDialog : Window
    {
        public string InputText => InputTextBox.Text;

        public TextInputDialog(string prompt, string defaultValue = "")
        {
            InitializeComponent();

            // Make sure the dialog uses the current theme
            if (ThemeManager.Current != null && ThemeManager.Current.CurrentTheme != null)
            {
                DataContext = ThemeManager.Current;
            }

            PromptText.Text = prompt;
            InputTextBox.Text = defaultValue;
            InputTextBox.SelectAll();
            InputTextBox.Focus();

            // Add ESC key handler
            PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
