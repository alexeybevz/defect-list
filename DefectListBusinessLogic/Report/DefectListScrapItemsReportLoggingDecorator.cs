using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class DefectListScrapItemsReportLoggingDecorator : ReportLoggingDecorator<bool>, IDefectListScrapItemsReport
    {
        private readonly DefectListScrapItemsReport _report;

        public DefectListScrapItemsReportLoggingDecorator(DefectListScrapItemsReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Отчет о браке ДСЕ";

        public async Task<bool> CreateAsync(IBomHeader bomHeader, IReadOnlyCollection<BomItem> data, string createdBy)
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