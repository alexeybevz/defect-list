using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllDefectToDecisionMapsQuery : DbConnectionPmControlRepositoryBase, IGetAllDefectToDecisionMapsQuery
    {
        public GetAllDefectToDecisionMapsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<MapDefectToDecision>> Execute()
        {
            var query = @"
                SELECT
	                  m.Id
	                , m.Defect
	                , m.Decision
	                , m.IsAllowCombine
	                , m.StateDetalsId AS Id
	                , s.StateName AS StateDetalsName
                    , gd.GroupDefectId AS Id
                    , gd.GroupDefectName
                FROM dbo.MapDefectToDecision m
                LEFT JOIN dbo.StateDetals s ON s.StateDetalsId = m.StateDetalsId
                LEFT JOIN dbo.GroupDefect gd ON gd.GroupDefectId = m.GroupDefectId
                ORDER BY gd.GroupDefectId, m.Defect";

            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<MapDefectToDecision, StateDetals, GroupDefect, MapDefectToDecision>(query,
                    (mapDefectToDecision, stateDetals, groupDefect) =>
                    {
                        mapDefectToDecision.StateDetals = stateDetals;
                        mapDefectToDecision.GroupDefect = groupDefect;
                        return mapDefectToDecision;
                    }, splitOn: "Id")).ToList();
            }
        }
    }
}