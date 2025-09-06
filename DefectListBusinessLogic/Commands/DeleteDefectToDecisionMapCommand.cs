using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteDefectToDecisionMapCommand : DbConnectionPmControlRepositoryBase, IDeleteDefectToDecisionMapCommand
    {
        public DeleteDefectToDecisionMapCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int id)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"DELETE FROM dbo.MapDefectToDecision WHERE Id = @Id";

                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    await db.ExecuteAsync(query, new { Id = id }, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}