using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Dtos;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class SummaryByOrdersReportLoggingDecorator : ReportLoggingDecorator<bool>, ISummaryByOrdersReport
    {
        private readonly SummaryByOrdersReport _report;

        public SummaryByOrdersReportLoggingDecorator(SummaryByOrdersReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Сводный отчет по заказам";

        public async Task<bool> CreateAsync(
            IEnumerable<BomHeader> bomHeaders,
            IEnumerable<IBomItem> items,
            IEnumerable<ProductDto> products,
            Dictionary<int, string> productsDistinctShopEntries,
            string createdBy)
        {
            return await CreateAsync(
                null,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeaders, items, products, productsDistinctShopEntries, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}