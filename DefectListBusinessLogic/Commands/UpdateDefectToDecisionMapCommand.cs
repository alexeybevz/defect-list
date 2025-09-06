using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateDefectToDecisionMapCommand : DbConnectionPmControlRepositoryBase, IUpdateDefectToDecisionMapCommand
    {
        public UpdateDefectToDecisionMapCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(MapDefectToDecision mapDefectToDecision)
        {
            var parameters = new
            {
                Defect = mapDefectToDecision.Defect,
                Decision = mapDefectToDecision.Decision,
                IsAllowCombine = mapDefectToDecision.IsAllowCombine,
                StateDetalsId = mapDefectToDecision.StateDetals.Id,
                GroupDefectId = mapDefectToDecision.GroupDefect.Id,
                Id = mapDefectToDecision.Id
            };

            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"UPDATE dbo.MapDefectToDecision
                              SET Defect = @Defect, Decision = @Decision, IsAllowCombine = @IsAllowCombine, StateDetalsId = @StateDetalsId, GroupDefectId = @GroupDefectId
                              WHERE Id = @Id";

                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    await db.ExecuteAsync(query, parameters, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}