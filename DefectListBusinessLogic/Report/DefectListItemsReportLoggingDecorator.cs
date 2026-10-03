using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using DefectListDomain.Services;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsReportLoggingDecorator : ReportLoggingDecorator<bool>, IDefectListItemsReport
    {
        public DefectListItemsReportLoggingDecorator(IReportExecutionLogger executionLogger) : base(executionLogger) { }

        protected override string ReportName => "Дефектовочная ведомость (pdf)";

        public async Task<bool> CreateAsync(
            IBomHeader bomHeader,
            DefectListItemsRptParm rptParm,
            string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy,
                async pathToReportDirectory =>
                    await CreateAsync(rptParm, pathToReportDirectory));
        }

        private async Task<bool> CreateAsync(DefectListItemsRptParm parm, string pathToReportDirectory)
        {
            var reportParmBuilder = new DefectListItemsRptParmBuilder(parm);

            var reportBuilder = new DefectListItemsRptBuilder(reportParmBuilder);
            var reportSender = new CrRptPdfSender<DefectListItems>(pathToReportDirectory);
            var reporter = new Reporter<DefectListItems>(reportBuilder, reportSender);

            var countReports = reporter.SendReports();
            return countReports > 0;
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}