using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class BomItemsFilterByDetalTypViewModel
    {
        public ObservableCollection<CheckBoxViewModel> DetalTypes { get; }

        public string SelectedDetalType => DetalTypes?.FirstOrDefault(x => x.IsChecked)?.Text ?? "все типы";

        public BomItemsFilterByDetalTypViewModel(DefectListItemViewModel vm)
        {
            DetalTypes = new ObservableCollection<CheckBoxViewModel>(new List<CheckBoxViewModel>()
            {
                new DetalTypFilterModeViewModel(vm, "все типы", true),
                new DetalTypFilterModeViewModel(vm, "дет"),
                new DetalTypFilterModeViewModel(vm, "сб.ед"),
                new DetalTypFilterModeViewModel(vm, "сб.ед + покуп"),
                new DetalTypFilterModeViewModel(vm, "покуп"),
                new DetalTypFilterModeViewModel(vm, "матер"),
                new DetalTypFilterModeViewModel(vm, "литье")
            });
        }
    }

    public class DetalTypFilterModeViewModel : CheckBoxViewModel
    {
        public DetalTypFilterModeViewModel(DefectListItemViewModel vm, string text, bool isChecked = false) : base(text, isChecked)
        {
            this.IsCheckedChanged += () => vm.BomItemsView.Refresh(); ;
        }
    }
}