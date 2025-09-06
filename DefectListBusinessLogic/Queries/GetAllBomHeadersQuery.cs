using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomHeadersQuery : DbConnectionPmControlRepositoryBase, IGetAllBomHeadersQuery
    {
        public GetAllBomHeadersQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<BomHeader>> Execute()
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<BomHeader, RootItem, BomHeader>(string.Format(SelectQuery, string.Empty),
                    (bomHeader, rootItem) =>
                    {
                        bomHeader.RootItem = rootItem;
                        bomHeader.StateInfo = new BomHeaderStateInfo(bomHeader.State, bomHeader.TotalRowsCount, bomHeader.FilledRowsCount);
                        return bomHeader;
                    })).ToList();
            }
        }

        public async Task<BomHeader> Execute(int id)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<BomHeader, RootItem, BomHeader>(string.Format(SelectQuery, "WHERE b.BomId = @id"),
                    (bomHeader, rootItem) =>
                    {
                        bomHeader.RootItem = rootItem;
                        bomHeader.StateInfo = new BomHeaderStateInfo(bomHeader.State, bomHeader.TotalRowsCount, bomHeader.FilledRowsCount);
                        return bomHeader;
                    }, new { id })).FirstOrDefault();
            }
        }

        private string SelectQuery
        {
            get
            {
                var query = @"WITH BomHeaderFilled AS (
	                        SELECT
		                          h.BomId
		                        , SUM(CASE WHEN IsShowItem = 1 THEN 1 ELSE 0 END) AS TotalRowsCount
		                        , SUM(
			                        CASE WHEN ISNULL(i.Defect, '') <> '' AND
					                          ISNULL(i.Decision, '') <> '' AND
                                              IsShowItem = 1 THEN 1
				                         ELSE 0 END
		                        ) AS FilledRowsCount
	                        FROM BomHeader h
	                        LEFT JOIN BomItem i ON i.BomId = h.BomId
	                        GROUP BY h.BomId
                        ), CreateDateFirstRouteMaps AS (
                            SELECT
                                  bi.BomId
                                , MIN(mp.CreateDate) AS MinCreateDate
                            FROM MapBomItemToRouteChart mp
                            LEFT JOIN BomItem bi ON bi.Id = mp.BomItemId
                            WHERE bi.BomId IS NOT NULL
                            GROUP BY bi.BomId
                        )
                        SELECT
                              b.BomId
                            , b.Orders
                            , b.SerialNumber
                            , b.SerialNumberAfterRepair
                            , b.IzdelQty
                            , b.StateDetalsId
                            , b.Comment
                            , f.TotalRowsCount
                            , f.FilledRowsCount
                            , b.DateOfSpecif
                            , b.DateOfTehproc
                            , b.DateOfMtrl
                            , b.DateOfPreparation
                            , b.CreatedBy
                            , ucrt.ActiveDirectoryCN AS CreatedByName
                            , b.CreateDate
                            , b.UpdatedBy
                            , uupd.ActiveDirectoryCN AS UpdatedByName
                            , b.RecordDate
                            , b.State
                            , b.Contract
                            , b.ContractDateOpen
                            , b.HeaderType
                            , fd.MinCreateDate AS CreateDateFirstRouteMap
                            , ri.Id
                            , ri.Izdels
                            , ri.Izdel
                            , ri.IzdelInitial
                            , ri.IzdelIma
                            , ri.IzdelTyp
                        FROM dbo.BomHeader b
                        INNER JOIN RootItem ri ON ri.Id = b.RootItemId
                        INNER JOIN BomHeaderFilled f ON f.BomId = b.BomId
                        LEFT JOIN dbo.Users ucrt ON ucrt.Login = b.CreatedBy
                        LEFT JOIN dbo.Users uupd ON uupd.Login = b.UpdatedBy
                        LEFT JOIN CreateDateFirstRouteMaps fd ON fd.BomId = b.BomId
                        {0}
                        ORDER BY BomId DESC";
                return query;
            }
        }
    }
}