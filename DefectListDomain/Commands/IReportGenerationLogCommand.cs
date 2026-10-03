using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IReportGenerationLogCommand
    {
        Task ExecuteAsync(string reportName, int? bomId, bool isSuccess, string error, string createdBy);
    }
}