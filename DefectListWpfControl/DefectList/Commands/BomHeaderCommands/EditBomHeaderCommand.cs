using System;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class EditBomHeaderCommand : AsyncCommandBase
    {
        private readonly EditBomHeaderViewModel _editBomHeaderViewModel;
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly CustomIdentity _user;

        public EditBomHeaderCommand(EditBomHeaderViewModel editBomHeaderViewModel, BomHeadersStore bomHeadersStore, CustomIdentity user)
        {
            _editBomHeaderViewModel = editBomHeaderViewModel;
            _bomHeadersStore = bomHeadersStore;
            _user = user;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var formViewModel = _editBomHeaderViewModel.BomHeaderDetailsFormViewModel;

            formViewModel.ErrorMessage = null;
            formViewModel.IsSubmitting = true;

            try
            {
                var bomHeader = new BomHeader()
                {
                    BomId = _editBomHeaderViewModel.BomHeader.BomId,
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
                    RecordDate = DateTime.Now,
                    UpdatedBy = _user.Name,
                    UpdatedByName = _user.CN,

                    CreateDateFirstRouteMap = _editBomHeaderViewModel.BomHeader.CreateDateFirstRouteMap,
                    CreateDate = _editBomHeaderViewModel.BomHeader.CreateDate,
                    CreatedBy = _editBomHeaderViewModel.BomHeader.CreatedBy,
                    CreatedByName = _editBomHeaderViewModel.BomHeader.CreatedByName,
                    StateDetalsId = _editBomHeaderViewModel.BomHeader.StateDetalsId,
                    State = _editBomHeaderViewModel.BomHeader.State,
                    StateInfo = _editBomHeaderViewModel.BomHeader.StateInfo,
                    FilledRowsCount = _editBomHeaderViewModel.BomHeader.FilledRowsCount,
                    TotalRowsCount = _editBomHeaderViewModel.BomHeader.FilledRowsCount
                };

                await _bomHeadersStore.Update(bomHeader);
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