using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Point = System.Windows.Point;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace ThemeForge.Themes.Dialogs
{
    /// <summary>
    /// Interaction logic for ColorPickerDialog.xaml
    /// </summary>
    public partial class ColorPickerDialog : Window
    {   
        private bool _updatingControls = false;
        private bool _mouseDown = false;
        private Window _eyeDropperWindow = null;
        private bool _isEyeDropperActive = false;
        private DispatcherTimer _eyeDropperTimer;

        // For the eyedropper functionality
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref POINT lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// The color selected by the user
        /// </summary>
        public Color SelectedColor { get; private set; }

        /// <summary>
        /// Creates a new color picker dialog
        /// </summary>
        /// <param name="initialColor">The initial color to display</param>
        public ColorPickerDialog(Color initialColor)
        {            
            InitializeComponent();

            // Initialize standard colors
            InitializeStandardColors();

            // Set the initial color
            SelectedColor = initialColor;
            CurrentColorPreview.Fill = new SolidColorBrush(initialColor);

            // Update the UI with the initial color
            UpdateControlsFromColor(initialColor);

            // Focus the color canvas
            Loaded += (s, e) => ColorCanvas.Focus();

            // Initialize eye dropper timer
            _eyeDropperTimer = new DispatcherTimer();
            _eyeDropperTimer.Interval = TimeSpan.FromMilliseconds(50);
            _eyeDropperTimer.Tick += EyeDropperTimer_Tick;

            // Add a global PreviewKeyDown handler to properly handle key events when eyedropper is active
            this.PreviewKeyDown += (s, e) =>
            {
                if (_isEyeDropperActive)
                {
                    if (e.Key == Key.Escape)
                    {
                        StopEyeDropper();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Space || e.Key == Key.Enter)
                    {
                        var pos = System.Windows.Forms.Control.MousePosition;
                        Color color = GetPixelColor(pos.X, pos.Y);
                        UpdateControlsFromColor(color);
                        StopEyeDropper();
                        e.Handled = true;
                    }
                }
            };
        }

        private void InitializeStandardColors()
        {            
            // Add standard colors to the panel
            var standardColors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Black),
                new SolidColorBrush(Colors.DimGray),
                new SolidColorBrush(Colors.Gray),
                new SolidColorBrush(Colors.DarkGray),
                new SolidColorBrush(Colors.Silver),
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Gainsboro),
                new SolidColorBrush(Colors.WhiteSmoke),
                new SolidColorBrush(Colors.White),

                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Firebrick),
                new SolidColorBrush(Colors.Crimson),
                new SolidColorBrush(Colors.Tomato),
                new SolidColorBrush(Colors.Coral),
                new SolidColorBrush(Colors.IndianRed),
                new SolidColorBrush(Colors.LightCoral),
                new SolidColorBrush(Colors.DarkSalmon),
                new SolidColorBrush(Colors.Salmon),
                new SolidColorBrush(Colors.LightSalmon),

                new SolidColorBrush(Colors.OrangeRed),
                new SolidColorBrush(Colors.DarkOrange),
                new SolidColorBrush(Colors.Orange),

                new SolidColorBrush(Colors.Gold),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.LightYellow),
                new SolidColorBrush(Colors.LemonChiffon),
                new SolidColorBrush(Colors.LightGoldenrodYellow),
                new SolidColorBrush(Colors.PapayaWhip),

                new SolidColorBrush(Colors.DarkGreen),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.ForestGreen),
                new SolidColorBrush(Colors.SeaGreen),
                new SolidColorBrush(Colors.MediumSeaGreen),
                new SolidColorBrush(Colors.LightSeaGreen),
                new SolidColorBrush(Colors.PaleGreen),
                new SolidColorBrush(Colors.SpringGreen),
                new SolidColorBrush(Colors.LawnGreen),
                new SolidColorBrush(Colors.Chartreuse),

                new SolidColorBrush(Colors.Navy),
                new SolidColorBrush(Colors.DarkBlue),
                new SolidColorBrush(Colors.MediumBlue),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.RoyalBlue),
                new SolidColorBrush(Colors.SteelBlue),
                new SolidColorBrush(Colors.DodgerBlue),
                new SolidColorBrush(Colors.DeepSkyBlue),
                new SolidColorBrush(Colors.LightBlue),
                new SolidColorBrush(Colors.SkyBlue),

                new SolidColorBrush(Colors.Purple),
                new SolidColorBrush(Colors.DarkMagenta),
                new SolidColorBrush(Colors.Magenta),
                new SolidColorBrush(Colors.MediumPurple),
                new SolidColorBrush(Colors.BlueViolet),
                new SolidColorBrush(Colors.Indigo),
                new SolidColorBrush(Colors.DarkViolet),
                new SolidColorBrush(Colors.Violet),
                new SolidColorBrush(Colors.Orchid),
                new SolidColorBrush(Colors.Plum),

                new SolidColorBrush(Colors.Brown),
                new SolidColorBrush(Colors.Chocolate),
                new SolidColorBrush(Colors.SaddleBrown),
                new SolidColorBrush(Colors.Sienna),
                new SolidColorBrush(Colors.Peru),
                new SolidColorBrush(Colors.DarkGoldenrod),
                new SolidColorBrush(Colors.Goldenrod),
                new SolidColorBrush(Colors.SandyBrown),
                new SolidColorBrush(Colors.Tan),
                new SolidColorBrush(Colors.Wheat),
                new SolidColorBrush(Colors.BurlyWood),

                new SolidColorBrush(Colors.Cyan),
                new SolidColorBrush(Colors.Teal),
                new SolidColorBrush(Colors.DarkCyan),
                new SolidColorBrush(Colors.LightCyan),
                new SolidColorBrush(Colors.Aquamarine),
                new SolidColorBrush(Colors.Turquoise),
                new SolidColorBrush(Colors.MediumTurquoise),
                new SolidColorBrush(Colors.DarkTurquoise),
                new SolidColorBrush(Colors.CadetBlue),
                new SolidColorBrush(Colors.Aqua)
            };

            StandardColorsPanel.ItemsSource = standardColors;
        }

        private void UpdateControlsFromColor(Color color)
        {            
            if (_updatingControls) return;
            _updatingControls = true;

            try
            {                
                // Update RGB text boxes
                RedTextBox.Text = color.R.ToString();
                GreenTextBox.Text = color.G.ToString();
                BlueTextBox.Text = color.B.ToString();

                // Update hex text box
                HexTextBox.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

                // Update color preview
                NewColorPreview.Fill = new SolidColorBrush(color);

                // Convert RGB to HSV
                var hsv = RgbToHsv(color.R, color.G, color.B);

                // Update HSV text boxes
                HueTextBox.Text = Math.Round(hsv.H).ToString();
                SaturationTextBox.Text = Math.Round(hsv.S * 100).ToString();
                ValueTextBox.Text = Math.Round(hsv.V * 100).ToString();

                // Update color canvas background (hue)
                ColorCanvas.Background = new SolidColorBrush(HsvToRgb(hsv.H, 1, 1));

                // Update color selector position
                double x = hsv.S * ColorCanvas.ActualWidth;
                double y = (1 - hsv.V) * ColorCanvas.ActualHeight;
                Canvas.SetLeft(ColorSelector, x);
                Canvas.SetTop(ColorSelector, y);

                // Update hue selector position
                double hueX = hsv.H / 360 * HueCanvas.ActualWidth;
                Canvas.SetLeft(HueSelectorCanvas, hueX);

                // Update selected color
                SelectedColor = color;
            }
            finally
            {                
                _updatingControls = false;
            }
        }

        private void UpdateColorFromPosition(Point position)
        {            
            if (_updatingControls) return;

            // Clamp position to the canvas bounds
            double x = Math.Clamp(position.X, 0, ColorCanvas.ActualWidth);
            double y = Math.Clamp(position.Y, 0, ColorCanvas.ActualHeight);

            // Update color selector position
            Canvas.SetLeft(ColorSelector, x - ColorSelector.Width / 2);
            Canvas.SetTop(ColorSelector, y - ColorSelector.Height / 2);

            // Calculate saturation and value from position
            double s = x / ColorCanvas.ActualWidth;
            double v = 1 - (y / ColorCanvas.ActualHeight);

            // Get the current hue from the canvas background
            var backgroundBrush = ColorCanvas.Background as SolidColorBrush;
            var hsv = RgbToHsv(backgroundBrush.Color.R, backgroundBrush.Color.G, backgroundBrush.Color.B);

            // Create the new color
            var color = HsvToRgb(hsv.H, s, v);

            // Update the controls
            UpdateColorFromRgb(color.R, color.G, color.B);
        }

        private void UpdateHueFromPosition(Point position)
        {            
            if (_updatingControls) return;

            // Clamp position to the canvas bounds
            double x = Math.Clamp(position.X, 0, HueCanvas.ActualWidth);

            // Update hue selector position
            Canvas.SetLeft(HueSelector, x - HueSelector.Width / 2);

            // Calculate hue from position
            double h = x / HueCanvas.ActualWidth * 360;

            // Update color canvas background
            ColorCanvas.Background = new SolidColorBrush(HsvToRgb(h, 1, 1));

            // Get the current saturation and value from the color selector position
            double s = (Canvas.GetLeft(ColorSelector) + ColorSelector.Width / 2) / ColorCanvas.ActualWidth;
            double v = 1 - ((Canvas.GetTop(ColorSelector) + ColorSelector.Height / 2) / ColorCanvas.ActualHeight);

            // Create the new color
            var color = HsvToRgb(h, s, v);

            // Update the controls
            UpdateColorFromRgb(color.R, color.G, color.B);
        }

        private void UpdateColorFromRgb(byte r, byte g, byte b)
        {            
            if (_updatingControls) return;
            _updatingControls = true;

            try
            {                
                // Create the color
                var color = Color.FromRgb(r, g, b);

                // Update RGB text boxes
                RedTextBox.Text = r.ToString();
                GreenTextBox.Text = g.ToString();
                BlueTextBox.Text = b.ToString();

                // Update hex text box
                HexTextBox.Text = $"#{r:X2}{g:X2}{b:X2}";

                // Update color preview
                NewColorPreview.Fill = new SolidColorBrush(color);

                // Update selected color
                SelectedColor = color;
            }
            finally
            {                
                _updatingControls = false;
            }
        }

        private void UpdateColorFromHex(string hex)
        {            
            if (_updatingControls) return;

            try
            {                
                // Remove # if present
                if (hex.StartsWith("#"))
                    hex = hex.Substring(1);

                // Ensure hex is 6 characters
                if (hex.Length != 6)
                    return;

                // Parse hex to RGB
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

                // Update the color
                UpdateControlsFromColor(Color.FromRgb(r, g, b));
            }
            catch
            {                
                // Ignore invalid hex values
            }
        }

        #region HSV/RGB Conversion

        private class HsvColor
        {            
            public double H { get; set; } // 0-360
            public double S { get; set; } // 0-1
            public double V { get; set; } // 0-1
        }

        private HsvColor RgbToHsv(byte r, byte g, byte b)
        {            
            double rd = r / 255.0;
            double gd = g / 255.0;
            double bd = b / 255.0;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double h = 0, s, v = max;

            double delta = max - min;

            if (max != 0)
                s = delta / max;
            else
            {
                s = 0;
                h = 0;
                return new HsvColor { H = h, S = s, V = v };
            }

            if (rd == max)
                h = (gd - bd) / delta;
            else if (gd == max)
                h = 2 + (bd - rd) / delta;
            else
                h = 4 + (rd - gd) / delta;

            h *= 60;
            if (h < 0)
                h += 360;

            return new HsvColor { H = h, S = s, V = v };
        }

        private Color HsvToRgb(double h, double s, double v)
        {            
            double r, g, b;

            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                h /= 60;
                int i = (int)Math.Floor(h);
                double f = h - i;
                double p = v * (1 - s);
                double q = v * (1 - s * f);
                double t = v * (1 - s * (1 - f));

                switch (i)
                {
                    case 0:
                        r = v; g = t; b = p;
                        break;
                    case 1:
                        r = q; g = v; b = p;
                        break;
                    case 2:
                        r = p; g = v; b = t;
                        break;
                    case 3:
                        r = p; g = q; b = v;
                        break;
                    case 4:
                        r = t; g = p; b = v;
                        break;
                    default: // case 5:
                        r = v; g = p; b = q;
                        break;
                }
            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        #endregion

        #region Event Handlers

        private void ColorCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDown = true;
                UpdateColorFromPosition(e.GetPosition(ColorCanvas));
                ColorCanvas.CaptureMouse();
            }
        }

        private void ColorCanvas_MouseMove(object sender, MouseEventArgs e)
        {            
            if (_mouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateColorFromPosition(e.GetPosition(ColorCanvas));
            }
        }

        private void ColorCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                ColorCanvas.ReleaseMouseCapture();
            }
        }

        private void HueCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDown = true;
                UpdateHueFromPosition(e.GetPosition(HueCanvas));
                HueCanvas.CaptureMouse();
            }
        }

        private void HueCanvas_MouseMove(object sender, MouseEventArgs e)
        {            
            if (_mouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateHueFromPosition(e.GetPosition(HueCanvas));
            }
        }

        private void HueCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                HueCanvas.ReleaseMouseCapture();
            }
        }

        private void StandardColor_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (sender is Border border && border.Background is SolidColorBrush brush)
            {
                UpdateControlsFromColor(brush.Color);
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {            
            // Allow only numbers
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void RgbTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {            
            if (_updatingControls) return;

            // Ensure all text boxes have valid values
            if (!byte.TryParse(RedTextBox.Text, out byte r))
                return;

            if (!byte.TryParse(GreenTextBox.Text, out byte g))
                return;

            if (!byte.TryParse(BlueTextBox.Text, out byte b))
                return;

            // Update the color
            UpdateControlsFromColor(Color.FromRgb(r, g, b));
        }

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {            
            if (_updatingControls) return;

            // Ensure hex has valid format
            var hex = HexTextBox.Text;
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6 && Regex.IsMatch(hex, "^[0-9A-Fa-f]+$"))
            {
                UpdateColorFromHex(hex);
            }
        }

        private void HsvTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updatingControls) return;

            // Ensure all text boxes have valid values
            if (!double.TryParse(HueTextBox.Text, out double h) || h < 0 || h > 360)
                return;

            if (!double.TryParse(SaturationTextBox.Text, out double s) || s < 0 || s > 100)
                return;

            if (!double.TryParse(ValueTextBox.Text, out double v) || v < 0 || v > 100)
                return;

            // Convert to 0-1 range for S and V
            s /= 100;
            v /= 100;

            // Update the color
            Color color = HsvToRgb(h, s, v);
            UpdateControlsFromColor(color);
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

        private void EyeDropperButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEyeDropperActive)
            {
                StopEyeDropper();
                return;
            }

            StartEyeDropper();
        }

        [DllImport("user32.dll")]
        private static extern bool SetCapture(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void StartEyeDropper()
        {
            _isEyeDropperActive = true;
            EyeDropperButton.Content = "Cancel Eye Dropper";

            // Get the virtual screen size (all monitors)
            var screenLeft = SystemParameters.VirtualScreenLeft;
            var screenTop = SystemParameters.VirtualScreenTop;
            var screenWidth = SystemParameters.VirtualScreenWidth;
            var screenHeight = SystemParameters.VirtualScreenHeight;

            // Create a floating preview that follows the cursor
            // We'll create a small, topmost window that just shows the preview
            _eyeDropperWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Width = 110,  // Just enough for the preview
                Height = 110, // Just enough for the preview
                Owner = this,
                Focusable = false
            };

            // Create a floating preview border
            Border previewBorder = new Border
            {
                Width = 100,
                Height = 100,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas previewCanvas = new Canvas
            {
                Width = 98,
                Height = 98
            };
            Image previewImage = new Image
            {
                Width = 98,
                Height = 98,
                Stretch = Stretch.None
            };
            previewCanvas.Children.Add(previewImage);
            previewBorder.Child = previewCanvas;

            // Crosshair
            Line horizontalLine = new Line
            {
                X1 = 40,
                Y1 = 49,
                X2 = 60,
                Y2 = 49,
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            Line verticalLine = new Line
            {
                X1 = 49,
                Y1 = 40,
                X2 = 49,
                Y2 = 60,
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            previewCanvas.Children.Add(horizontalLine);
            previewCanvas.Children.Add(verticalLine);

            _eyeDropperWindow.Content = previewBorder;
            _eyeDropperWindow.Show();

            // We'll handle mouse events globally through hooks instead of window events
            IntPtr hwnd = new WindowInteropHelper(_eyeDropperWindow).Handle;

            // Setup global mouse event handling
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);

            // Start the timer to update preview position and color
            _eyeDropperTimer.Tick -= EyeDropperTimer_Tick;
            _eyeDropperTimer.Tick += (s, e) =>
            {
                if (!_isEyeDropperActive) return;

                // Get the cursor position
                var pos = System.Windows.Forms.Control.MousePosition;

                // Update the window position to follow the cursor
                _eyeDropperWindow.Left = pos.X + 20;
                _eyeDropperWindow.Top = pos.Y + 20;

                // Keep window within screen bounds
                if (_eyeDropperWindow.Left + _eyeDropperWindow.Width > screenLeft + screenWidth)
                    _eyeDropperWindow.Left = screenLeft + screenWidth - _eyeDropperWindow.Width;

                if (_eyeDropperWindow.Top + _eyeDropperWindow.Height > screenTop + screenHeight)
                    _eyeDropperWindow.Top = screenTop + screenHeight - _eyeDropperWindow.Height;

                // Update the preview
                UpdateEyeDropperPreview(previewImage, pos.X, pos.Y);
            };
            _eyeDropperTimer.Start();

            // Hook into mouse events globally
            SetCapture(hwnd);

            // Setup initial position
            var initPos = System.Windows.Forms.Control.MousePosition;
            _eyeDropperWindow.Left = initPos.X + 20;
            _eyeDropperWindow.Top = initPos.Y + 20;
            UpdateEyeDropperPreview(previewImage, initPos.X, initPos.Y);

            // Listen for keyboard input in the main window
            this.KeyDown += EyeDropper_KeyDown;
        }

        private void EyeDropper_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEyeDropperActive) return;

            if (e.Key == Key.Escape)
            {
                StopEyeDropper();
                e.Handled = true;
            }
            else if (e.Key == Key.Space || e.Key == Key.Return)
            {
                var pos = System.Windows.Forms.Control.MousePosition;
                Color color = GetPixelColor(pos.X, pos.Y);
                UpdateControlsFromColor(color);
                StopEyeDropper();
                e.Handled = true;
            }
        }

        // Windows message hook
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;

            if (msg == WM_LBUTTONDOWN && _isEyeDropperActive)
            {
                var pos = System.Windows.Forms.Control.MousePosition;
                Color color = GetPixelColor(pos.X, pos.Y);
                UpdateControlsFromColor(color);
                StopEyeDropper();
                handled = true;
            }
            else if (msg == WM_RBUTTONDOWN && _isEyeDropperActive)
            {
                StopEyeDropper();
                handled = true;
            }

            return IntPtr.Zero;
        }

        // Helper to update the preview image and color
        private void UpdateEyeDropperPreview(Image previewImage, int x, int y)
        {
            if (previewImage == null)
                return;

            int zoom = 6;
            int size = 16;

            try
            {
                using (var bmp = new System.Drawing.Bitmap(size, size))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(x - size / 2, y - size / 2, 0, 0, new System.Drawing.Size(size, size));
                    }
                    var hBitmap = bmp.GetHbitmap();
                    try
                    {
                        var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(size, size));
                        var transform = new ScaleTransform(zoom, zoom);
                        var transformedBitmap = new TransformedBitmap(bitmapSource, transform);
                        previewImage.Source = transformedBitmap;
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                    }
                }
                // Update color preview
                Color color = GetPixelColor(x, y);
                NewColorPreview.Fill = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during screen capture
                System.Diagnostics.Debug.WriteLine($"Error updating eyedropper preview: {ex.Message}");
            }
        }

        private void StopEyeDropper()
        {
            if (!_isEyeDropperActive) return;

            _isEyeDropperActive = false;
            EyeDropperButton.Content = "Pick Screen Color";

            // Release mouse capture
            ReleaseCapture();

            // Remove keyboard handler
            this.KeyDown -= EyeDropper_KeyDown;

            // Stop the timer
            _eyeDropperTimer.Stop();

            // Close the window
            if (_eyeDropperWindow != null)
            {
                try
                {
                    // Remove hook if window still exists
                    IntPtr hwnd = new WindowInteropHelper(_eyeDropperWindow).Handle;
                    HwndSource source = HwndSource.FromHwnd(hwnd);
                    if (source != null)
                    {
                        source.RemoveHook(WndProc);
                    }
                }
                catch { /* Ignore errors during cleanup */ }

                _eyeDropperWindow.Close();
                _eyeDropperWindow = null;
            }
        }

        private void EyeDropperTimer_Tick(object sender, EventArgs e)
        {
            if (_eyeDropperWindow == null || !_isEyeDropperActive)
                return;

            // Get the cursor position
            POINT cursorPos = new POINT();
            GetCursorPos(ref cursorPos);

            // Get the preview image
            Image previewImage = ((Canvas)((Border)((Grid)_eyeDropperWindow.Content).Children[0]).Child).Children[0] as Image;

            // Update the preview and color
            UpdateEyeDropperPreview(previewImage, cursorPos.X, cursorPos.Y);

            // Get the pixel color at the cursor position
            Color color = GetPixelColor(cursorPos.X, cursorPos.Y);

            // Update the color preview in the dialog
            NewColorPreview.Fill = new SolidColorBrush(color);
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private Color GetPixelColor(int x, int y)
        {
            IntPtr desktopDC = GetDC(IntPtr.Zero); // Get the DC of the entire screen
            try
            {
                uint pixel = GetPixel(desktopDC, x, y);
                byte r = (byte)((pixel >> 0) & 0xff);
                byte g = (byte)((pixel >> 8) & 0xff);
                byte b = (byte)((pixel >> 16) & 0xff);
                return Color.FromRgb(r, g, b);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, desktopDC);
            }
        }


        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        // Helper class for screen capture
        private class VisualHost : UIElement
        {
            protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;

        #endregion
    }
}
