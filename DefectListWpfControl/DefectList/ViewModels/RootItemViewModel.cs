using System.Windows.Input;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class RootItemViewModel
    {
        public RootItem RootItem { get; }
        public ICommand RootItemDoubleClickCommand { get; }

        public RootItemViewModel(RootItem rootItem, ICommand rootItemDoubleClickCommand)
        {
            RootItem = rootItem;
            RootItemDoubleClickCommand = rootItemDoubleClickCommand;
        }
    }
}