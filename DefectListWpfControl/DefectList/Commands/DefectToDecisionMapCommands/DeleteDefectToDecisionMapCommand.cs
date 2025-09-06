using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class DeleteDefectToDecisionMapCommand : AsyncCommandBase
    {
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private readonly SelectedDefectToDecisionMapStore _selectedDefectToDecisionMapStore;

        public DeleteDefectToDecisionMapCommand(DefectToDecisionMapsStore defectToDecisionMapsStore, SelectedDefectToDecisionMapStore selectedDefectToDecisionMapStore)
        {
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            _selectedDefectToDecisionMapStore = selectedDefectToDecisionMapStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                if (_selectedDefectToDecisionMapStore.SelectedDefectToDecisionMap == null)
                    return;

                if (MessageBox.Show($"Вы действительно хотите выполнить удаление выделенной связки дефект-решение?", 
                    "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                await _defectToDecisionMapsStore.Delete(_selectedDefectToDecisionMapStore.SelectedDefectToDecisionMap.Id);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при удалении связки дефект-решение:\n" + e.Message);
            }
        }
    }
}