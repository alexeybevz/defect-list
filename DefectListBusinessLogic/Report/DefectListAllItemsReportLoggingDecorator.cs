using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class DefectListAllItemsReportLoggingDecorator : ReportLoggingDecorator<bool>, IDefectListAllItemsReport
    {
        private readonly DefectListAllItemsReport _report;

        public DefectListAllItemsReportLoggingDecorator(DefectListAllItemsReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Дефектовочная ведомость (xlsx)";

        public async Task<bool> CreateAsync(IBomHeader bomHeader, IEnumerable<BomItem> data, string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeader, data, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}