using System;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IReportExecutionLogger
    {
        Task<ReportExecutionResult<TResult>> CreateAsync<TResult>(
            string reportName,
            string reportDirectory,
            string createdBy,
            int? bomId,
            Func<string, Task<TResult>> createReport,
            Func<TResult, bool> isSuccess = null
        );
    }
}