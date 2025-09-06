using System.Windows.Input;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Commands.BomHeaderCommands;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class EditBomHeaderViewModel : ViewModel
    {
        public string FormName => $"Изменение дефектовочной ведомости № {BomHeader.BomId}";

        public BomHeader BomHeader { get; }
        public BomHeaderDetailsFormViewModel BomHeaderDetailsFormViewModel { get; }

        public EditBomHeaderViewModel(ProductsStore productsStore, BomHeader bomHeader, BomHeadersStore bomHeadersStore,  RootItemsStore rootItemsStore, CustomIdentity user)
        {
            BomHeader = bomHeader;

            ICommand submitCommand = new EditBomHeaderCommand(this, bomHeadersStore, user);
            BomHeaderDetailsFormViewModel = new BomHeaderDetailsFormViewModel(productsStore, rootItemsStore, submitCommand)
            {
                Orders = bomHeader.Orders,
                Contract = bomHeader.Contract,
                ContractDateOpen = bomHeader.ContractDateOpen,
                SerialNumber = bomHeader.SerialNumber,
                SerialNumberAfterRepair = bomHeader.SerialNumberAfterRepair,
                RootItem = bomHeader.RootItem,
                Comment = bomHeader.Comment,
                DateOfSpecif = bomHeader.DateOfSpecif,
                DateOfPreparation = bomHeader.DateOfPreparation,
                HeaderType = bomHeader.HeaderType,
            };
        }
    }
}