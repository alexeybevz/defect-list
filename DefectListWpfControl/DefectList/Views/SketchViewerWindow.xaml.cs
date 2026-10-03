using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DefectListWpfControl.DefectList.Views
{
    public partial class SketchViewerWindow : Window
    {
        private double _zoom = 1.0;
        private const double ZoomStep = 0.25;
        private const double ZoomMin = 0.25;
        private const double ZoomMax = 4.0;
        // Зум колесом чуть мельче шага кнопок — удобнее для плавного управления
        private const double WheelStep = 0.1;

        public SketchViewerWindow(string filePath, string title)
        {
            InitializeComponent();

            try
            {
                var uri = new Uri(filePath, UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                // OnLoad — файл читается сразу, дескриптор освобождается,
                // чтобы файл можно было переименовать/удалить пока окно открыто
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                SketchImage.Source = bitmap;
                Title = title;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить файл эскиза:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) => SetZoom(_zoom + ZoomStep);
        private void ZoomOut_Click(object sender, RoutedEventArgs e) => SetZoom(_zoom - ZoomStep);
        private void ZoomReset_Click(object sender, RoutedEventArgs e) => SetZoom(1.0);

        // Колесо мыши
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Без Ctrl — стандартная вертикальная прокрутка ScrollViewer
            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                return;

            // С Ctrl — зум; перехватываем событие, чтобы ScrollViewer не прокручивался
            e.Handled = true;

            double delta = e.Delta > 0 ? WheelStep : -WheelStep;
            SetZoom(_zoom + delta);
        }

        // Общий метод применения масштаба
        private void SetZoom(double zoom)
        {
            _zoom = Math.Max(ZoomMin, Math.Min(ZoomMax, zoom));

            ImageScale.ScaleX = _zoom;
            ImageScale.ScaleY = _zoom;

            ZoomLabel.Text = $"{(int)Math.Round(_zoom * 100)}%";
        }
    }
}