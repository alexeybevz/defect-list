using System.Threading.Tasks;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Report
{
    public class MeasurementMapReportLoggingDecorator : ReportLoggingDecorator<bool>, IMeasurementMapReport
    {
        private readonly MeasurementMapReport _report;

        public MeasurementMapReportLoggingDecorator(MeasurementMapReport report, IReportExecutionLogger executionLogger) : base(executionLogger)
        {
            _report = report;
        }

        protected override string ReportName => "Карта измерения";

        public async Task<bool> CreateAsync(
            MeasurementMap map,
            BomItem bomItem,
            BomHeader bomHeader,
            string createdBy)
        {
            return await CreateAsync(
                bomHeader.BomId,
                createdBy,
                async pathToReportDirectory =>
                    await _report.CreateAsync(map, bomItem, bomHeader, pathToReportDirectory));
        }

        protected override bool IsSuccess(bool result)
        {
            return result;
        }
    }
}