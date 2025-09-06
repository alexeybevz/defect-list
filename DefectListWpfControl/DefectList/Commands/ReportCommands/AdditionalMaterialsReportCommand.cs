using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListBusinessLogic.Report;
using DefectListDomain.ExternalData;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class AdditionalMaterialsReportCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly IGetAllAuxiliaryMaterialDtoQuery _getAllAuxiliaryMaterialDtoQuery;
        private readonly IGetAllOgmetMatlDtoQuery _getAllOgmetMatlDtoQuery;

        public AdditionalMaterialsReportCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore, IGetAllAuxiliaryMaterialDtoQuery getAllAuxiliaryMaterialDtoQuery, IGetAllOgmetMatlDtoQuery getAllOgmetMatlDtoQuery)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _getAllAuxiliaryMaterialDtoQuery = getAllAuxiliaryMaterialDtoQuery;
            _getAllOgmetMatlDtoQuery = getAllOgmetMatlDtoQuery;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            IReportDirectory reportDirectory = new ReportDirectory(_bomItemViewModel.UserIdentity.Name);
            try
            {
                reportDirectory.Create();

                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList();

                var reportBuilder = new AdditionalMaterialsReport(_getAllAuxiliaryMaterialDtoQuery, _getAllOgmetMatlDtoQuery);
                reportBuilder.Create(_bomItemViewModel.BomHeader, bomItems, reportDirectory.PathReportDirectory);

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
        }
    }
}