using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commons
{
    public class BomItemFieldPermissionToIsReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return true;
            }
            else
            {
                var str = parameter as string;
                var vm = (value as BomItemViewModel)?.DefectListItemViewModel;

                FieldPermission obj = null;
                var b = vm?.FieldsPermissions.TryGetValue(str, out obj) ?? false;

                if (!b)
                    return true;

                return !obj.CanEdit;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}