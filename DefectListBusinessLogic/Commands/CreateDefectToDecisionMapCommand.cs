using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateDefectToDecisionMapCommand : DbConnectionPmControlRepositoryBase, ICreateDefectToDecisionMapCommand
    {
        public CreateDefectToDecisionMapCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(MapDefectToDecision mapDefectToDecision)
        {
            var parameters = new
            {
                Defect = mapDefectToDecision.Defect,
                Decision = mapDefectToDecision.Decision,
                IsAllowCombine = mapDefectToDecision.IsAllowCombine,
                StateDetalsId = mapDefectToDecision.StateDetals.Id,
                GroupDefectId = mapDefectToDecision.GroupDefect.Id
            };

            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"INSERT INTO dbo.MapDefectToDecision (Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId)
                              VALUES (@Defect, @Decision, @IsAllowCombine, @StateDetalsId, @GroupDefectId);
                              SELECT SCOPE_IDENTITY()";

                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    mapDefectToDecision.Id = await db.QuerySingleAsync<int>(query, parameters, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}