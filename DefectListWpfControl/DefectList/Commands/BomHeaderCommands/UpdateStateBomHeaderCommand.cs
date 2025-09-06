using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class UpdateStateBomHeaderCommand : AsyncCommandBase
    {
        private readonly BomHeaderViewModel _bomHeaderViewModel;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly string _userName;
        private readonly BomHeaderState _bomHeaderState;

        public UpdateStateBomHeaderCommand(BomHeaderViewModel bomHeaderViewModel, BomHeadersStore bomHeadersStore, string userName, BomHeaderState bomHeaderState)
        {
            _bomHeaderViewModel = bomHeaderViewModel;
            _bomHeadersStore = bomHeadersStore;
            _userName = userName;
            _bomHeaderState = bomHeaderState;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var bomHeader = _bomHeaderViewModel.BomHeader;

                if (bomHeader == null)
                    return;

                if (bomHeader.StateInfo.IsWaitApproved && _bomHeaderState == BomHeaderState.Approved)
                {
                    if (MessageBox.Show("Вы уверены, что хотите изменить статус ведомости на \"Утверждено\"?\n\n" +
                                        "После выполнения этого действия ведомость смогут редактировать только пользователи с расширенными правами.\n\n" +
                                        "Обычные пользователи, которым доступно заполнение, смогут только просматривать ведомость без возможности изменить ее.", "Внимание",
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;

                    await UpdateState(bomHeader, _bomHeaderState);
                    return;
                }

                if (bomHeader.StateInfo.IsApproved && _bomHeaderState == BomHeaderState.WorkInProgress)
                {
                    if (MessageBox.Show("Вы уверены, что хотите установить статус ведомости \"В работе\"?\n\n" +
                                        "После выполнения этого действия появится возможность ее корректировки любыми пользователями, а также корректировки состава.", "Внимание",
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;

                    await UpdateState(bomHeader, _bomHeaderState);
                    return;
                }

                if (bomHeader.StateInfo.IsApproved && _bomHeaderState == BomHeaderState.Closed)
                {
                    if (MessageBox.Show("Вы уверены, что хотите изменить статус ведомости на \"Закрыто\"?\n\n" +
                                        "После выполнения этого действия исключается возможность ее корректировки любыми пользователями.\n\n" +
                                        "При необходимости, отдел ИТ на основе заявки на портале тех.поддержки может перевести статус ведомости в \"Утверждено\" " +
                                        "для предоставления возможности работать с ней пользователям с расширенными правами.", "Внимание",
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;

                    await UpdateState(bomHeader, _bomHeaderState);
                    return;
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
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser;
        }
    }
}