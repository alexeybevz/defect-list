using System;
using System.Threading.Tasks;
using DefectListDomain.Services;
using ReporterDomain.Services.CreateReportService;

namespace DefectListBusinessLogic.Report
{
    public abstract class ReportLoggingDecorator<TResult>
    {
        private readonly IReportExecutionLogger _executionLogger;

        protected ReportLoggingDecorator(IReportExecutionLogger executionLogger)
        {
            _executionLogger = executionLogger;
        }

        protected abstract string ReportName { get; }

        protected virtual bool IsSuccess(TResult result) => true;

        protected async Task<TResult> CreateAsync(int? bomId, string createdBy, Func<string, Task<TResult>> createReport)
        {
            var reportDirectory = new ReportDirectory(createdBy);

            try
            {
                reportDirectory.Create();

                var execution = await _executionLogger.CreateAsync(
                    ReportName,
                    reportDirectory.PathReportDirectory,
                    createdBy,
                    bomId,
                    createReport,
                    IsSuccess);

                if (execution.IsSuccess)
                    reportDirectory.Open();

                return execution.Result;
            }
            finally
            {
                reportDirectory.DeleteIfEmpty();
            }
        }
    }
}