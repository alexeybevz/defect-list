using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class OpenChangesFinalDecisionReportFormCommand : AsyncCommandBase
    {
        private readonly BomItemsStore _bomItemsStore;
        private readonly ProductsStore _productsStore;
        private readonly string _userName;

        public OpenChangesFinalDecisionReportFormCommand(BomItemsStore bomItemsStore, ProductsStore productsStore, string userName)
        {
            _bomItemsStore = bomItemsStore;
            _productsStore = productsStore;
            _userName = userName;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var vm = new ChangesFinalDecisionReportViewModel(_bomItemsStore, _productsStore, _userName);
            var form = new ChangesFinalDecisionReportWindow() { DataContext = vm };
            form.ShowDialog();
        }
    }
}