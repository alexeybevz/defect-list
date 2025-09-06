using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class DefectListHeaderFilterModeViewModel : ViewModel
    {
        private readonly DefectListHeaderViewModel _defectListHeaderViewModel;
        private readonly BomHeadersStore _bomHeadersStore;
        public ObservableCollection<FilterModeViewModel> FilterModes { get; }

        public FilterModeViewModel IsSelectedFilterMode
        {
            get
            {
                return FilterModes.FirstOrDefault(x => x.IsChecked) ?? FilterModes.FirstOrDefault();
            }
        }

        public DefectListHeaderFilterModeViewModel(DefectListHeaderViewModel defectListHeaderViewModel, BomHeadersStore bomHeadersStore)
        {
            _defectListHeaderViewModel = defectListHeaderViewModel;
            _bomHeadersStore = bomHeadersStore;
            FilterModes = new ObservableCollection<FilterModeViewModel>()
            {
                new FilterModeViewModel(OnBomHeadersFiltered, "Стандартный фильтр", true),
                new FilterModeViewModel(OnBomHeadersFilteredBySerialNumberFull, "Фильтр по серийным номерам ДСЕ (полное совпадение)"),
                new FilterModeViewModel(OnBomHeadersFilteredBySerialNumberPartial, "Фильтр по серийным номерам ДСЕ (частичное совпадение)"),
            };
        }

        private bool OnBomHeadersFiltered(object obj)
        {
            var bomHeader = (BomHeaderViewModel)obj;
            var filterText = _defectListHeaderViewModel.FilterString?.Trim();
            var isShowCompletedHeaders = _defectListHeaderViewModel.ShowCompletedHeadersViewModel.IsChecked || bomHeader.State == 0;

            if (string.IsNullOrWhiteSpace(filterText))
                return isShowCompletedHeaders;

            var filterTextUpper = filterText.ToUpper();
            var result =
                bomHeader.RootItem.Izdel.Contains(filterTextUpper) ||
                bomHeader.RootItem.IzdelIma.Contains(filterTextUpper) ||
                bomHeader.SerialNumber != null && bomHeader.SerialNumber.ToUpper().Contains(filterTextUpper) ||
                bomHeader.Orders != null && bomHeader.Orders.ToUpper().Contains(filterTextUpper) ||
                bomHeader.Comment != null && bomHeader.Comment.ToUpper().Contains(filterTextUpper) ||
                bomHeader.StateInfo.ApprovalState != null && bomHeader.StateInfo.ApprovalState.ToUpper().Contains(filterTextUpper);

            return result && isShowCompletedHeaders;
        }

        private bool OnBomHeadersFilteredBySerialNumberFull(object obj)
        {
            var bomHeader = (BomHeaderViewModel)obj;
            var filterText = _defectListHeaderViewModel.FilterString?.Trim();
            var isShowCompletedHeaders = _defectListHeaderViewModel.ShowCompletedHeadersViewModel.IsChecked || bomHeader.State == 0;

            if (string.IsNullOrWhiteSpace(filterText))
                return isShowCompletedHeaders;

            var filterTextUpper = filterText.ToUpper();

            IEnumerable<int> bomIds;
            _bomHeadersStore.BomItemsWithSerialNumbers.TryGetValue(filterTextUpper, out bomIds);

            return (bomIds ?? new List<int>()).Contains(bomHeader.BomId) && isShowCompletedHeaders;
        }

        private bool OnBomHeadersFilteredBySerialNumberPartial(object obj)
        {
            var bomHeader = (BomHeaderViewModel)obj;
            var filterText = _defectListHeaderViewModel.FilterString?.Trim();
            var isShowCompletedHeaders = _defectListHeaderViewModel.ShowCompletedHeadersViewModel.IsChecked || bomHeader.State == 0;

            if (string.IsNullOrWhiteSpace(filterText))
                return isShowCompletedHeaders;

            var filterTextUpper = filterText.ToUpper();
            var bomIds = _bomHeadersStore.BomItemsWithSerialNumbers.Where(x => x.Key.ToUpper().Contains(filterTextUpper)).SelectMany(x => x.Value).ToList();
            return bomIds.Contains(bomHeader.BomId) && isShowCompletedHeaders;
        }
    }

    public class FilterModeViewModel : CheckBoxViewModel
    {
        public Predicate<object> Filter { get; }

        public FilterModeViewModel(Predicate<object> filter, string text, bool isChecked = false) : base(text, isChecked)
        {
            Filter = filter;
        }
    }
}
