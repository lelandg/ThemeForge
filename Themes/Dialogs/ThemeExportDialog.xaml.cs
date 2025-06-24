using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ThemeForge.Themes.Dialogs
{
    /// <summary>
    /// Interaction logic for ThemeExportDialog.xaml
    /// </summary>
    public partial class ThemeExportDialog : Window
    {
        /// <summary>
        /// The type of theme export to perform
        /// </summary>
        public ThemeExportType ExportType
        {
            get
            {
                if (ExportWindowRadio.IsChecked == true)
                    return ThemeExportType.Window;

                if (ExportMessageBoxRadio.IsChecked == true)
                    return ThemeExportType.MessageBox;

                return ThemeExportType.Both;
            }
        }

        /// <summary>
        /// Creates a new theme export dialog
        /// </summary>
        public ThemeExportDialog()
        {
            InitializeComponent();

            PreviewKeyDown += PreviewKey; 
        }

        private void PreviewKey(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    OkButton_Click(sender, e);
                    break;
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
