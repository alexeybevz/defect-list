using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using MessageBox = System.Windows.MessageBox;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class LoadBomItemsToBomHeaderCommand : AsyncCommandBase
    {
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly string _userName;

        public LoadBomItemsToBomHeaderCommand(
            BomHeadersStore bomHeadersStore,
            SelectedBomHeaderStore selectedBomHeaderStore,
            string userName)
        {
            _bomHeadersStore = bomHeadersStore;
            _selectedBomHeaderStore = selectedBomHeaderStore;
            _userName = userName;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var bomHeader = _selectedBomHeaderStore.SelectedBomHeader;

            if (bomHeader == null)
                return;

            if (bomHeader.StateInfo.IsClosed)
            {
                MessageBox.Show("Дефектовочная ведомость находится в состоянии Закрыто. Выполнение операции запрещено");
                return;
            }

            await Task.Run(async () =>
            {
                try
                {
                    if (bomHeader.TotalRowsCount > 0)
                    {
                        if (MessageBox.Show("Произойдет удаление дефектовочной ведомости и ее замена только на плановый состав.\n" +
                                            "Вы уверены, что хотите выполнить это действие?", "Внимание",
                            MessageBoxButton.YesNo) == MessageBoxResult.No)
                            return;
                    }
                    else
                    {
                        if (MessageBox.Show($"Произойдет загрузка состава изделия {bomHeader.RootItem.Izdel} в дефектовочную ведомость № {bomHeader.BomId}.\n" +
                                            "Вы уверены, что хотите выполнить это действие?", "Внимание",
                            MessageBoxButton.YesNo) == MessageBoxResult.No)
                            return;
                    }

                    await _bomHeadersStore.LoadBomItemsToBomHeader(bomHeader, _userName);

                    MessageBox.Show($"Состав изделия {bomHeader.RootItem.Izdel} успешно загружен в дефектовочную ведомость № {bomHeader.BomId}");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Произошла ошибка при загрузке состава изделия {bomHeader.RootItem.Izdel} в дефектовочную ведомость № {bomHeader.BomId}:\n\n" +
                                    e.Message);
                }
                finally
                {
                    await _bomHeadersStore.Load(bomHeader.BomId);
                }
            });
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}