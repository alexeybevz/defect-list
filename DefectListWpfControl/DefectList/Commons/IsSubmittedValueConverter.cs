using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.Commons
{
    public class IsSubmittedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : BomItemsConstantsStore.IsSubmittedDict[(IsBomItemSubmitted)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = BomItemsConstantsStore.IsSubmittedDict.FirstOrDefault(x => x.Value == value).Key;
            return result;
        }
    }
}