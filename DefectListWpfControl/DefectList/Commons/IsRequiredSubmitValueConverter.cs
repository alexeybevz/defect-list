using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.Commons
{
    public class IsRequiredSubmitValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : BomItemsConstantsStore.IsRequiredSubmitDict[(IsBomItemRequiredSubmit)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BomItemsConstantsStore.IsRequiredSubmitDict.FirstOrDefault(x => x.Value == value).Key;
        }
    }
}