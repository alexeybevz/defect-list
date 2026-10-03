using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class ReportGenerationLogCommand : DbConnectionPmControlRepositoryBase, IReportGenerationLogCommand
    {
        public ReportGenerationLogCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task ExecuteAsync(string reportName, int? bomId, bool isSuccess, string error, string createdBy)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        const string sql =
                            @"INSERT INTO ReportGenerationLog (ReportName, BomId, IsSuccess, Error, CreatedBy)
                              VALUES (@ReportName, @BomId, @IsSuccess, @Error, @CreatedBy);";

                        await db.ExecuteAsync(sql, new { reportName, bomId, isSuccess, error, createdBy }, tran);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}