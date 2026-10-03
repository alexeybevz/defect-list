using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using DefectListDomain.Services;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsReclamationReportLoggingDecorator : ReportLoggingDecorator<bool>, IDefectListItemsReclamationReport
    {
        public DefectListItemsReclamationReportLoggingDecorator(IReportExecutionLogger executionLogger) : base(executionLogger) { }

        protected override string ReportName => "Дефектовочная ведомость - рекламация (pdf)";

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

            var reportBuilder = new DefectListItemsReclamationRptBuilder(reportParmBuilder);
            var reportSender = new CrRptPdfSender<DefectListItemsReclamation>(pathToReportDirectory);
            var reporter = new Reporter<DefectListItemsReclamation>(reportBuilder, reportSender);

            var countReports = reporter.SendReports();
            return countReports > 0;
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}