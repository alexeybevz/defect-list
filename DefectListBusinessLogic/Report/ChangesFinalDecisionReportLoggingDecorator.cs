using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class ChangesFinalDecisionReportLoggingDecorator : ReportLoggingDecorator<bool>, IChangesFinalDecisionReport
    {
        private readonly ChangesFinalDecisionReport _report;

        public ChangesFinalDecisionReportLoggingDecorator(ChangesFinalDecisionReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Журнал изменения окончательного решения";

        public async Task<bool> CreateAsync(
            IReadOnlyCollection<FinalDecisionChanging> data,
            IReadOnlyDictionary<int, string> productsDistinctShopEntries,
            string createdBy)
        {
            return await CreateAsync(
                null,
                createdBy, 
                async pathToReportDirectory
                    => await _report.CreateAsync(data, productsDistinctShopEntries, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}