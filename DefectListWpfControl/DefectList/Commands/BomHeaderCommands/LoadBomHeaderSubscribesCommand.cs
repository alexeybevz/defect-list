using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class LoadBomHeaderSubscribesCommand : AsyncCommandBase
    {
        private readonly BomHeaderSubscribersStore _bomHeaderSubscribersStore;

        public LoadBomHeaderSubscribesCommand(BomHeaderSubscribersStore bomHeaderSubscribersStore)
        {
            _bomHeaderSubscribersStore = bomHeaderSubscribersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                await _bomHeaderSubscribersStore.Load();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}