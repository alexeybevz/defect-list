using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class AuxiliaryMaterialsReportLoggingDecorator : ReportLoggingDecorator<bool>, IAuxiliaryMaterialsReport
    {
        private readonly AuxiliaryMaterialsReport _report;

        public AuxiliaryMaterialsReportLoggingDecorator(AuxiliaryMaterialsReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Плановый расход вспом. матер. (прямые расходы)";

        public async Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyCollection<BomItem> bomItemsView,
            string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy, 
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeader, bomItemsView, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}