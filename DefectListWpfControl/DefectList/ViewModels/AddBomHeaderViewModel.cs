using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.BomHeaderCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class AddBomHeaderViewModel : ViewModel
    {
        public string FormName => "Создание дефектовочной ведомости";

        public BomHeaderDetailsFormViewModel BomHeaderDetailsFormViewModel { get; }

        public AddBomHeaderViewModel(ProductsStore productsStore, BomHeadersStore bomHeadersStore, RootItemsStore rootItemsStore, CustomIdentity user)
        {
            ICommand submitCommand = new AddBomHeaderCommand(this, bomHeadersStore, user);
            BomHeaderDetailsFormViewModel = new BomHeaderDetailsFormViewModel(productsStore, rootItemsStore, submitCommand);
        }

        public AddBomHeaderViewModel(ProductsStore productsStore, BomHeadersStore bomHeadersStore, RootItemsStore rootItemsStore, CustomIdentity user, BomHeader bomHeader)
        {
            ICommand submitCommand = new AddBomHeaderCommand(this, bomHeadersStore, user);
            BomHeaderDetailsFormViewModel = new BomHeaderDetailsFormViewModel(productsStore, rootItemsStore, submitCommand)
            {
                Contract = bomHeader.Contract,
                ContractDateOpen = bomHeader.ContractDateOpen,
                RootItem = new RootItem()
                {
                    Id = bomHeader.RootItem.Id,
                    Izdels = bomHeader.RootItem.Izdels,
                    Izdel = bomHeader.RootItem.Izdel,
                    IzdelIma = bomHeader.RootItem.IzdelIma,
                    IzdelTyp = bomHeader.RootItem.IzdelTyp,
                    IzdelInitial = bomHeader.RootItem.IzdelInitial
                }
            };
        }
    }
}