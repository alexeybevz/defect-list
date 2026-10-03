using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class GetItemInfoByAllProductsReportLoggingDecorator : ReportLoggingDecorator<bool>, IGetItemInfoByAllProductsReport
    {
        private readonly GetItemInfoByAllProductsReport _report;

        public GetItemInfoByAllProductsReportLoggingDecorator(GetItemInfoByAllProductsReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Анализ номенклатуры";

        public async Task<bool> CreateAsync(IEnumerable<IBomHeader> bomHeaders, IEnumerable<IBomItem> bomItems, string createdBy)
        {
            return await CreateAsync(
                null,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeaders, bomItems, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}