using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ThemeForge
{
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(BitmapSource imageSource)
        {
            InitializeComponent();
            ImageDisplay.Source = imageSource;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
