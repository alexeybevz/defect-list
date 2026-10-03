using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.CreatingReports;
using DefectListWpfControl.DefectList.Commons;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ChangesFinalDecisionReportCommand : AsyncCommandBase
    {
        private readonly ChangesFinalDecisionReportViewModel _changesFinalDecisionReportViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly ProductsStore _productsStore;
        private readonly string _userName;

        public ChangesFinalDecisionReportCommand(
            ChangesFinalDecisionReportViewModel changesFinalDecisionReportViewModel,
            BomItemsStore bomItemsStore,
            ProductsStore productsStore,
            string userName)
        {
            _changesFinalDecisionReportViewModel = changesFinalDecisionReportViewModel;
            _bomItemsStore = bomItemsStore;
            _productsStore = productsStore;
            _userName = userName;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _changesFinalDecisionReportViewModel.IsLoading = true;

            try
            {
                _changesFinalDecisionReportViewModel.ExecutingStatus =
                    "Получение данных об изменениях решения в ДВ ...";
                var data = (await _bomItemsStore.GetFinalDecisionChangings(
                    _changesFinalDecisionReportViewModel.StartDate,
                    _changesFinalDecisionReportViewModel.EndDate,
                    ComboBoxHelper.ToFilterValue(_changesFinalDecisionReportViewModel.SelectedDetalTyp))).ToList();

                _changesFinalDecisionReportViewModel.ExecutingStatus = "Получение данных о расцеховке из АСУП ...";
                var productsDistinctShopEntries = await _productsStore.GetAllDistinctShopEntries();

                _changesFinalDecisionReportViewModel.ExecutingStatus = "Формирование excel файла ...";

                var reportBuilder = DefectListIocKernel.Get<IChangesFinalDecisionReport>();
                var isReportCreated = await reportBuilder.CreateAsync(data, productsDistinctShopEntries, _userName);
                if (isReportCreated)
                {
                    _changesFinalDecisionReportViewModel.IsLoading = false;
                    MessageBox.Show("Отчет сформирован.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                _changesFinalDecisionReportViewModel.IsLoading = false;
                _changesFinalDecisionReportViewModel.ExecutingStatus = null;
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && !_changesFinalDecisionReportViewModel.IsLoading;
        }
    }
}