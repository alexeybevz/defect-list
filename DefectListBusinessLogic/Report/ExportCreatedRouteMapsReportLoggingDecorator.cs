using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Dtos;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class ExportCreatedRouteMapsReportLoggingDecorator : ReportLoggingDecorator<bool>, IExportCreatedRouteMapsReport
    {
        private readonly ExportCreatedRouteMapsReport _report;

        public ExportCreatedRouteMapsReportLoggingDecorator(ExportCreatedRouteMapsReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Отчет о созданных МК за сессию";

        public async Task<bool> CreateAsync(IBomHeader bomHeader, IReadOnlyList<RouteMapDto> items, string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(bomHeader.RootItem.Izdel, items, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}