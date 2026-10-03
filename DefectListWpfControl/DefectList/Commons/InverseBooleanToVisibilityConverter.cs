using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DefectListWpfControl.DefectList.Commons
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = !(value is bool && (bool) value);
            return visible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility) value != Visibility.Visible;
        }
    }
}