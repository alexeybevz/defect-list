using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class ExecuteDoubleClickOnBomHeaderCommand : CommandBase
    {
        private readonly OpenedBomHeadersStore _openedBomHeadersStore;

        public ExecuteDoubleClickOnBomHeaderCommand(OpenedBomHeadersStore openedBomHeadersStore)
        {
            _openedBomHeadersStore = openedBomHeadersStore;
        }

        public override void Execute(object parameter = null)
        {
            var bomHeaderViewModel = parameter as BomHeaderViewModel;
            if (bomHeaderViewModel != null)
            {
                var cmd = new DoubleClickBomHeaderCommand(_openedBomHeadersStore, bomHeaderViewModel);
                cmd.Execute(null);
            }
        }
    }
}