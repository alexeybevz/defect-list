using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class DefectListItemsChangesReportLoggingDecorator : ReportLoggingDecorator<bool>, IDefectListItemsChangesReport
    {
        private readonly DefectListItemsChangesReport _report;

        public DefectListItemsChangesReportLoggingDecorator(DefectListItemsChangesReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Журнал изменений по ДВ";

        public async Task<bool> CreateAsync(
            IBomHeader bomHeader,
            IReadOnlyCollection<BomItem> bomItems,
            IReadOnlyCollection<BomItemLog> bomItemsLogs,
            string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeader, bomItems, bomItemsLogs, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}