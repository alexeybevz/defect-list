using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DefectListWpfControl.ViewModelImplement
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class WindowBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || (bool) value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}