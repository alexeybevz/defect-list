using System;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class DoubleClickBomHeaderCommand : CommandBase
    {
        private readonly OpenedBomHeadersStore _openedBomHeadersStore;
        private readonly BomHeaderViewModel _bomHeaderViewModel;

        public DoubleClickBomHeaderCommand(OpenedBomHeadersStore openedBomHeadersStore, BomHeaderViewModel bomHeaderViewModel)
        {
            _openedBomHeadersStore = openedBomHeadersStore;
            _bomHeaderViewModel = bomHeaderViewModel;
        }

        public override void Execute(object parameter = null)
        {
            var bomHeader = _bomHeaderViewModel;

            if (bomHeader == null)
                return;

            if (bomHeader.TotalRowsCount == 0)
            {
                MessageBox.Show($"Не загружен состав ремонтного изделия по заказу '{bomHeader.Orders}'. \n\n" +
                                "Для загрузки состава в меню нажмите 'Действие' -> 'Загрузить состав в дефектовочную ведомость'.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var form = _openedBomHeadersStore.GetOpenedForm(bomHeader.BomId);
                if (form == null)
                {
                    form = _openedBomHeadersStore.Add(bomHeader.BomId);
                    form.Show();
                }
                else
                {
                    form.Focus();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}