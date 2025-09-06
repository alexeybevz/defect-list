using System;
using System.Globalization;
using System.Windows.Data;

namespace DefectListWpfControl.DefectList.Commons
{
    public class FloatNumberValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "0" : value.ToString().Replace(",", ".");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            string str = value.ToString();
            if (string.IsNullOrEmpty(str))
                return 0;

            if (str.EndsWith("."))
                str = str.Substring(0, str.Length - 1);

            float number;
            var result = float.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
            return result ? number : 0;
        }
    }
}