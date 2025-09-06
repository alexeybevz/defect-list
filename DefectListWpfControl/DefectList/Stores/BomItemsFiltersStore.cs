using System;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Stores
{
    public static class BomItemsFiltersStore
    {
        public interface IBomItemFilter
        {
            bool Apply(BomItemViewModel bomItem, string filterText);
        }

        public class DetalFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.Detal.Contains(filterText.ToUpper());
            }
        }

        public class DetalImaFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.DetalIma.Contains(filterText.ToUpper());
            }
        }

        public class StructureNumberFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                if (string.IsNullOrEmpty(filterText))
                    return true;

                var filterParsed = filterText.Split('@');
                if (filterParsed.Length == 1)
                    return bomItem.StructureNumber.IndexOf(filterParsed[0], StringComparison.Ordinal) == 0;

                foreach (var s in filterParsed)
                {
                    var result = bomItem.StructureNumber.IndexOf(s, StringComparison.Ordinal) == 0;
                    if (result)
                        return true;
                }

                return false;
            }
        }

        public class DetalTypFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.DetalTyp == filterText;
            }
        }

        public class IsRequiredSubmitTextFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.IsRequiredSubmitText == filterText;
            }
        }

        public class IsSubmittedTextFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.IsSubmittedText == filterText;
            }
        }

        public class SerialNumberFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.SerialNumber?.Contains(filterText) ?? false;
            }
        }

        public class RepairMethodNameFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.RepairMethodName == filterText;
            }
        }

        public class InitialDecisionFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.Decision?.ToLower().Equals(filterText.ToLower()) ?? false;
            }
        }

        public class FinalDecisionFilter : IBomItemFilter
        {
            public bool Apply(BomItemViewModel bomItem, string filterText)
            {
                return bomItem.FinalDecision?.ToLower().Equals(filterText.ToLower()) ?? false;
            }
        }
    }
}