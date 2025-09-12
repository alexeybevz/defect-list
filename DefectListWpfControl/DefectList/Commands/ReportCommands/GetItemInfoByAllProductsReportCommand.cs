using System;
using System.Linq;
using System.Windows;
using DefectListBusinessLogic.Report;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;
using ReporterBusinessLogic;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class GetItemInfoByAllProductsReportCommand : CommandBase
    {
        private readonly BomHeadersStore _bomHeadersStore;
        private readonly BomItemsStore _bomItemsStore;
        private readonly ProductsStore _productsStore;
        private readonly string _userName;

        public GetItemInfoByAllProductsReportCommand(BomHeadersStore bomHeadersStore, BomItemsStore bomItemsStore, ProductsStore productsStore, string userName)
        {
            _bomHeadersStore = bomHeadersStore;
            _bomItemsStore = bomItemsStore;
            _productsStore = productsStore;
            _userName = userName;
        }

        public override void Execute(object parameter = null)
        {
            var selectNewItemWindow = new SelectItemWindow(_productsStore, false);
            var viewModel = ((SelectItemViewModel)selectNewItemWindow.DataContext);
            viewModel.NewBomItemSelected += async (newBomItem, product) =>
            {
                IReportDirectory reportDirectory = new ReportDirectory(_userName);
                try
                {
                    reportDirectory.Create();

                    if (product == null)
                        product = await _productsStore.GetProductByDetals(SpecifKeyCreator.CreateKey(newBomItem.Detal));
                    if (product == null)
                        throw new ArgumentException($"Указанная номенклатура '{newBomItem.Detal}' не найдена в классификаторе");

                    var bomHeaders = _bomHeadersStore.BomHeaders;
                    var bomItems = (await _bomItemsStore.GetBomItemsByProductId(product.Id)).ToList();

                    if (!bomItems.Any())
                    {
                        MessageBox.Show("Указанная номенклатура не найдена в дефектовочных ведомостях");
                        return;
                    }

                    var getItemInfoByAllProductsReport = new GetItemInfoByAllProductsReport();
                    getItemInfoByAllProductsReport.Create(bomHeaders, bomItems, reportDirectory.PathReportDirectory);

                    MessageBox.Show("Отчет сформирован.");
                    reportDirectory.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    reportDirectory.DeleteIfEmpty();
                }
            };

            selectNewItemWindow.ShowDialog();
        }
    }
}