using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DefectListWpfControl.DefectList.Commons
{
    public static class ComboBoxHelper
    {
        public const string AllOption = "Все";

        public static ObservableCollection<string> WithAllOption(IReadOnlyList<string> items)
        {
            var result = new ObservableCollection<string>() { AllOption };
            foreach (var item in items)
            {
                result.Add(item);
            }

            return result;
        }

        public static string ToFilterValue(string selected) =>
            selected == AllOption ? null : selected;
    }
}