using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class UpdateStateFromClosedToApprovedBomHeaderCommand : AsyncCommandBase
    {
        private readonly SelectedBomHeaderStore _selectedBomHeaderStore;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly string _userName;
        private readonly BomHeaderState _bomHeaderState;

        public UpdateStateFromClosedToApprovedBomHeaderCommand(SelectedBomHeaderStore selectedBomHeaderStore, BomHeadersStore bomHeadersStore, string userName, BomHeaderState bomHeaderState)
        {
            _selectedBomHeaderStore = selectedBomHeaderStore;
            _bomHeadersStore = bomHeadersStore;
            _userName = userName;
            _bomHeaderState = bomHeaderState;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var bomHeader = _selectedBomHeaderStore.SelectedBomHeader;

                if (bomHeader == null)
                    return;

                if (bomHeader.StateInfo.IsClosed && _bomHeaderState == BomHeaderState.Approved)
                {
                    if (MessageBox.Show("Произойдет изменение состояния ДВ с Закрыто на Утверждено.\n" +
                                        "Вы уверены, что хотите выполнить это действие?", "Внимание",
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;


                    await UpdateState(bomHeader, _bomHeaderState);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при изменении состояния ДВ:\n" + e.Message);
            }
        }

        private async Task UpdateState(BomHeader bomHeader, BomHeaderState bomHeaderState)
        {
            var newbomHeader = new BomHeader()
            {
                BomId = bomHeader.BomId,
                Orders = bomHeader.Orders,
                Contract = bomHeader.Contract,
                ContractDateOpen = bomHeader.ContractDateOpen,
                SerialNumber = bomHeader.SerialNumber,
                SerialNumberAfterRepair = bomHeader.SerialNumberAfterRepair,
                RootItem = bomHeader.RootItem,
                IzdelQty = bomHeader.IzdelQty,
                Comment = bomHeader.Comment,
                DateOfSpecif = bomHeader.DateOfSpecif,
                DateOfTehproc = bomHeader.DateOfTehproc,
                DateOfMtrl = bomHeader.DateOfMtrl,
                DateOfPreparation = bomHeader.DateOfPreparation,
                HeaderType = bomHeader.HeaderType,
                CreateDate = bomHeader.CreateDate,
                CreatedBy = bomHeader.CreatedBy,
                CreateDateFirstRouteMap = bomHeader.CreateDateFirstRouteMap,
                StateDetalsId = bomHeader.StateDetalsId,
                FilledRowsCount = bomHeader.TotalRowsCount,
                TotalRowsCount = bomHeader.FilledRowsCount,

                RecordDate = DateTime.Now,
                UpdatedBy = _userName,
                State = bomHeaderState,
                StateInfo = new BomHeaderStateInfo(bomHeaderState, bomHeader.TotalRowsCount, bomHeader.FilledRowsCount),
            };

            await _bomHeadersStore.UpdateState(newbomHeader);
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsAdmin;
        }
    }
}