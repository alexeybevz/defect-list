using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class SummaryByOrdersReportCommand : AsyncCommandBase
    {
        private readonly BomHeadersStore _bomHeaderStore;
        private readonly BomItemsStore _bomItemsStore;
        private readonly ProductsStore _productsStore;
        private readonly string _userName;

        public SummaryByOrdersReportCommand(BomHeadersStore bomHeaderStore, BomItemsStore bomItemsStore, ProductsStore productsStore, string userName)
        {
            _bomHeaderStore = bomHeaderStore;
            _bomItemsStore = bomItemsStore;
            _productsStore = productsStore;
            _userName = userName;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var window = new ChoiceOrdersWindow(_bomHeaderStore.BomHeaders);
                window.ShowDialog();

                var selectedBomHeaders = window.SelectBomHeaders;

                if (selectedBomHeaders == null)
                    return;

                var bomHeaders = selectedBomHeaders.ToList();
                if (!bomHeaders.Any())
                {
                    MessageBox.Show("Нет выбранных заказов для формирования отчета");
                    return;
                }

                var itemsAll = new List<IBomItem>();
                foreach (var selectedBomHeader in bomHeaders)
                {
                    itemsAll.AddRange(await _bomItemsStore.GetBomItemIsShowedView(selectedBomHeader.BomId));
                }

                if (!itemsAll.Any())
                {
                    MessageBox.Show("Выбранные дефектовочные ведомости не содержат заполненной информации");
                    return;
                }

                var products = await _productsStore.GetAllDesignSpecifications();

                var productsDistinctShopEntries = await _productsStore.GetAllDistinctShopEntries();

                var report = DefectListIocKernel.Get<ISummaryByOrdersReport>();
                var isReportCreated = await report.CreateAsync(bomHeaders, itemsAll, products, productsDistinctShopEntries, _userName);

                if (isReportCreated)
                    MessageBox.Show("Отчет сформирован.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}