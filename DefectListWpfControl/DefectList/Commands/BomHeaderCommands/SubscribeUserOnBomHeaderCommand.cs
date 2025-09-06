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
    public class SubscribeUserOnBomHeaderCommand : AsyncCommandBase
    {
        private readonly BomHeaderViewModel _bomHeaderModel;
        private readonly BomHeaderSubscribersStore _bomHeaderSubscribersStore;

        public SubscribeUserOnBomHeaderCommand(BomHeaderViewModel bomHeaderModel, BomHeaderSubscribersStore bomHeaderSubscribersStore)
        {
            _bomHeaderModel = bomHeaderModel;
            _bomHeaderSubscribersStore = bomHeaderSubscribersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (MessageBox.Show($"Вы уверены, что хотите получать уведомления по электронной почте об изменениях в дефектовочной ведомости на заказ '{_bomHeaderModel.Orders}'?", "Внимание",
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

                await _bomHeaderSubscribersStore.Subscribe(bomHeaderSubscriber);
                _bomHeaderModel.IsUserSubscribeOnBomHeader = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}