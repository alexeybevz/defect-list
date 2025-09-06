using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomItemLogsQuery : DbConnectionPmControlRepositoryBase, IGetAllBomItemLogsQuery
    {
        public GetAllBomItemLogsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<BomItemLog>> ExecuteByBomId(int bomId)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"SELECT log.*, u.ActiveDirectoryCN AS CreatedByName
                              FROM dbo.BomItemLog log
                              LEFT JOIN dbo.BomItem bi ON bi.Id = log.BomItemId
                              LEFT JOIN dbo.Users u ON u.Login = log.CreatedBy
                              WHERE bi.BomId = @BomId";

                return (await db.QueryAsync<BomItemLog>(query, new { BomId = bomId })).ToList();
            }
        }

        public async Task<IEnumerable<BomItemLog>> ExecuteByBomItemId(int bomItemId)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"SELECT log.*, u.ActiveDirectoryCN AS CreatedByName
                              FROM dbo.BomItemLog log
                              LEFT JOIN dbo.Users u ON u.Login = log.CreatedBy
                              WHERE log.BomItemId = @BomItemId
                              ORDER BY log.Id";

                return (await db.QueryAsync<BomItemLog>(query, new { BomItemId = bomItemId })).ToList();
            }
        }

        public async Task<IEnumerable<FinalDecisionChanging>> FinalDecisionChangings(DateTime startDate, DateTime endDate)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<FinalDecisionChanging>(GetQueryFinalDecisionChangings(), new { startDate, endDate = endDate.AddDays(1).Date })).ToList();
            }
        }

        private string GetQueryFinalDecisionChangings()
        {
            return @"with data as (
	                      select t1.*, (select top 1 Id from BomItemLog where Id > t1.Id and BomItemDocId = t1.BomItemDocId) AS NextId
	                      from BomItemLog t1
                      )
                      select
	                        bh.BomId
	                      , bh.Orders
	                      , ri.Izdel
	                      , bi.Detal
	                      , bi.DetalIma
	                      , bi.DetalTyp
	                      , bi.QtyMnf
	                      , bi.DetalUm
	                      , d.FinalDecision
	                      , b.FinalDecision AS NextFinalDecision
	                      , STUFF( (select ', ' + RouteChart_Number from MapBomItemToRouteChart m where m.BomItemId = d.BomItemId for xml path ('')), 1, 2, '') AS Nomgodurs
	                      , b.CreateDate
	                      , u.ActiveDirectoryCN AS CreatedByName
                      from data d
                      left join BomItemLog b on b.Id = d.NextId
                      left join BomItem bi on bi.Id = d.BomItemId
                      left join BomHeader bh on bh.BomId = bi.BomId
                      left join RootItem ri on ri.Id = bh.RootItemId
                      left join Users u on u.Login = b.CreatedBy
                      where d.FinalDecision is not null and b.FinalDecision is not null and ISNULL(d.FinalDecision,'') != ISNULL(b.FinalDecision,'')
                        and d.CreateDate between @startDate and @endDate
                      order by b.CreateDate desc";
        }
    }
}