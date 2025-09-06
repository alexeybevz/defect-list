using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class DeleteBomHeaderCommand : AsyncCommandBase
    {
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly BomHeadersStore _bomHeadersStore;

        public DeleteBomHeaderCommand(SelectedBomHeaderStore selectedBomHeaderStore, BomHeadersStore bomHeadersStore)
        {
            _selectedBomHeaderStore = selectedBomHeaderStore;
            _bomHeadersStore = bomHeadersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                if (_selectedBomHeaderStore.SelectedBomHeader == null)
                    return;

                if (_selectedBomHeaderStore.SelectedBomHeader.StateInfo.IsClosed)
                {
                    MessageBox.Show("Дефектовочная ведомость находится в состоянии Закрыто. Выполнение операции запрещено");
                    return;
                }

                if (MessageBox.Show($"Вы действительно хотите выполнить удаление дефектовочной ведомости № {_selectedBomHeaderStore.SelectedBomHeader.BomId}?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                await _bomHeadersStore.Delete(_selectedBomHeaderStore.SelectedBomHeader.BomId);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при удалении ДВ:\n" + e.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}