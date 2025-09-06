using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class UnSubscribeUserOnBomHeaderCommand : AsyncCommandBase
    {
        private readonly BomHeaderViewModel _bomHeaderModel;
        private BomHeaderSubscribersStore _bomHeaderSubscribersStore;

        public UnSubscribeUserOnBomHeaderCommand(BomHeaderViewModel bomHeaderModel, BomHeaderSubscribersStore bomHeaderSubscribersStore)
        {
            _bomHeaderModel = bomHeaderModel;
            _bomHeaderSubscribersStore = bomHeaderSubscribersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (MessageBox.Show($"Вы уверены, что больше не хотите получать уведомления по электронной почте об изменениях в дефектовочной ведомости '{_bomHeaderModel.Orders}'?", "Внимание",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                var user = Thread.CurrentPrincipal.Identity as CustomIdentity;
                var bomHeaderSubscriber = new BomHeaderSubscriber()
                {
                    BomId = _bomHeaderModel.BomId,
                    UserId = user.UserId
                };

                await _bomHeaderSubscribersStore.UnSubscribe(bomHeaderSubscriber);
                _bomHeaderModel.IsUserSubscribeOnBomHeader = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}