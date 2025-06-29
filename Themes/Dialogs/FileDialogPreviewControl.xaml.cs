using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ThemeForge.Themes.Dialogs
{
    public partial class FileDialogPreviewControl : System.Windows.Controls.UserControl
    {
        private const int MaxTextPreviewLength = 4096;
        private readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
        private readonly string[] TextExtensions = { ".txt", ".cs", ".xaml", ".xml", ".json", ".html", ".htm", ".css", ".js", ".py", ".cpp", ".c", ".h" };
        private readonly string[] ModelExtensions = { ".obj", ".stl", ".ply" };

        public FileDialogPreviewControl()
        {
            InitializeComponent();
        }

        public void ShowPreview(string filePath)
        {
            // Reset visibility
            NoPreviewText.Visibility = Visibility.Visible;
            PreviewImage.Visibility = Visibility.Collapsed;
            TextPreviewScroller.Visibility = Visibility.Collapsed;
            ModelPreview.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

                // Handle images
                if (Array.Exists(ImageExtensions, ext => ext == extension))
                {
                    ShowImagePreview(filePath);
                }
                // Handle text files
                else if (Array.Exists(TextExtensions, ext => ext == extension))
                {
                    ShowTextPreview(filePath);
                }
                // Handle 3D models
                else if (Array.Exists(ModelExtensions, ext => ext == extension))
                {
                    // For now, just show placeholder
                    NoPreviewText.Text = "3D Model Preview\n(Not implemented)";
                }
            }
            catch
            {
                // If any error occurs, fall back to no preview
                NoPreviewText.Visibility = Visibility.Visible;
            }
        }

        private void ShowImagePreview(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(filePath);
                bitmap.DecodePixelWidth = 300; // Limit the size to improve performance
                bitmap.EndInit();
                bitmap.Freeze(); // Freeze for cross-thread access

                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                NoPreviewText.Visibility = Visibility.Collapsed;
            }
            catch
            {
                NoPreviewText.Text = "Unable to load image preview";
            }
        }

        private void ShowTextPreview(string filePath)
        {
            try
            {
                // Read file content with limit to prevent loading huge files
                string content;
                using (var reader = new StreamReader(filePath))
                {
                    char[] buffer = new char[MaxTextPreviewLength];
                    int read = reader.Read(buffer, 0, MaxTextPreviewLength);
                    content = new string(buffer, 0, read);

                    if (reader.Peek() >= 0)
                    {
                        content += "\n[File content truncated...]";
                    }
                }

                TextPreview.Text = content;
                TextPreviewScroller.Visibility = Visibility.Visible;
                NoPreviewText.Visibility = Visibility.Collapsed;
            }
            catch
            {
                NoPreviewText.Text = "Unable to load text preview";
            }
        }
    }
}