using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System;
using DefectListDomain.Models;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class AddBomHeaderCommand : AsyncCommandBase
    {
        private readonly AddBomHeaderViewModel _addBomHeaderViewModel;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly CustomIdentity _user;

        public AddBomHeaderCommand(AddBomHeaderViewModel addBomHeaderViewModel, BomHeadersStore bomHeadersStore, CustomIdentity user)
        {
            _addBomHeaderViewModel = addBomHeaderViewModel;
            _bomHeadersStore = bomHeadersStore;
            _user = user;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var formViewModel = _addBomHeaderViewModel.BomHeaderDetailsFormViewModel;

            formViewModel.ErrorMessage = null;
            formViewModel.IsSubmitting = true;

            try
            {
                var now = DateTime.Now;

                var bomHeader = new BomHeader()
                {
                    Orders = formViewModel.Orders?.Trim(),
                    Contract = formViewModel.Contract?.Trim(),
                    ContractDateOpen = formViewModel.ContractDateOpen?.Date ?? DateTime.MinValue,
                    SerialNumber = formViewModel.SerialNumber?.Trim(),
                    SerialNumberAfterRepair = formViewModel.SerialNumberAfterRepair?.Trim(),
                    RootItem = formViewModel.RootItem,
                    IzdelQty = 1,
                    Comment = formViewModel.Comment?.Trim(),
                    DateOfSpecif = formViewModel.DateOfSpecif?.Date ?? DateTime.MaxValue,
                    DateOfTehproc = formViewModel.DateOfSpecif?.Date ?? DateTime.MaxValue,
                    DateOfMtrl = formViewModel.DateOfSpecif?.Date ?? DateTime.MaxValue,
                    DateOfPreparation = formViewModel.DateOfPreparation,
                    HeaderType = formViewModel.HeaderType?.Trim(),
                    CreateDate = now,
                    CreatedBy = _user.Name,
                    CreatedByName = _user.CN,
                    RecordDate = now,
                    UpdatedBy = _user.Name,
                    UpdatedByName = _user.CN,

                    CreateDateFirstRouteMap = null,
                    StateDetalsId = 1,
                    State = 0,
                    StateInfo = new BomHeaderStateInfo(0, 0, 0),
                    FilledRowsCount = 0,
                    TotalRowsCount = 0
                };

                await _bomHeadersStore.Add(bomHeader);
            }
            catch (Exception ex)
            {
                formViewModel.ErrorMessage = "Ошибка: " + ex.Message;
            }
            finally
            {
                formViewModel.IsSubmitting = false;
            }
        }
    }
}