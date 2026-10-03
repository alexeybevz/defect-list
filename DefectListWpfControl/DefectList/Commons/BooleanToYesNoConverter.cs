using System;
using System.Globalization;
using System.Windows.Data;

namespace DefectListWpfControl.DefectList.Commons
{
    // Конвертер bool → "Да"/"Нет" для колонок DataGrid
    public class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value is bool;
            return result ? (bool) value ? "Да" : "Нет" : "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}