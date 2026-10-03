using System;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class ReportExecutionLogger : IReportExecutionLogger
    {
        private readonly IReportGenerationLogCommand _reportGenerationLogCommand;

        public ReportExecutionLogger(IReportGenerationLogCommand reportGenerationLogCommand)
        {
            _reportGenerationLogCommand = reportGenerationLogCommand;
        }

        public async Task<ReportExecutionResult<TResult>> CreateAsync<TResult>(
            string reportName,
            string reportDirectory,
            string createdBy,
            int? bomId,
            Func<string, Task<TResult>> createReport,
            Func<TResult, bool> isSuccess = null)
        {
            string error = null;
            var success = false;
            TResult result = default(TResult);

            try
            {
                result = await createReport(reportDirectory);
                success = isSuccess?.Invoke(result) ?? true;

                if (!success)
                    error = "Report creation returned false";
            }
            catch (Exception e)
            {
                error = e.Message;
                throw;
            }
            finally
            {
                try
                {
                    await _reportGenerationLogCommand.ExecuteAsync(reportName, bomId, success, error, createdBy);
                }
                catch
                {
                    // ignored
                }
            }

            return new ReportExecutionResult<TResult>(result, success, error);
        }
    }
}