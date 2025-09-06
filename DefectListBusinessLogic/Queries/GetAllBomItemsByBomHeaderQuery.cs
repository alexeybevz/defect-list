using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using Dapper;
using DefectListDomain.Queries;
using DefectListDomain.Models;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomItemsByBomHeaderQuery : DbConnectionPmControlRepositoryBase, IGetAllBomItemsByBomHeaderQuery
    {
        private readonly IGetAllIsExpensiveClassifierQuery _getAllIsExpensiveClassifierQuery;
        public GetAllBomItemsByBomHeaderQuery(IDbConnectionFactory dbConnectionFactory, IGetAllIsExpensiveClassifierQuery getAllIsExpensiveClassifierQuery) : base(dbConnectionFactory)
        {
            _getAllIsExpensiveClassifierQuery = getAllIsExpensiveClassifierQuery;
        }

        public async Task<IEnumerable<BomItem>> Execute(int bomId)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    return await DefectListItemsBaseMethod(db, transaction, "WHERE bi.BomId = @BomId", new { BomId = bomId });
                }
            }
        }

        public async Task<IEnumerable<BomItem>> Execute(int bomId, IDbConnection db, IDbTransaction transaction)
        {
            return await DefectListItemsBaseMethod(db, transaction, "WHERE bi.BomId = @BomId", new { BomId = bomId });
        }

        public async Task<BomItem> ExecuteById(int id)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    return (await DefectListItemsBaseMethod(db, transaction, "WHERE bi.Id = @BomItemId", new { BomItemId = id })).FirstOrDefault();
                }
            }
        }

        public async Task<BomItem> ExecuteById(int id, IDbConnection db, IDbTransaction transaction)
        {
            return (await DefectListItemsBaseMethod(db, transaction, "WHERE bi.Id = @BomItemId", new { BomItemId = id })).FirstOrDefault();
        }

        public async Task<IEnumerable<BomItem>> ExecuteByProductId(int productId)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    return await DefectListItemsBaseMethod(db, transaction, "WHERE bi.ProductID = @ProductID", new { ProductID = productId });
                }
            }
        }

        private async Task<IEnumerable<BomItem>> DefectListItemsBaseMethod(IDbConnection db, IDbTransaction transaction, string wherePredicate, object param)
        {
            var expensiveItems = _getAllIsExpensiveClassifierQuery.Execute().ToDictionary(k => k, v => v);

            string query = GetMainSelectQuery(wherePredicate);
            var lookup = new Dictionary<int, BomItem>();
            await db.QueryAsync<BomItem, MapBomItemToRouteChart, BomItem>(query,
                (bomItem, mapBomItemToRouteChart) =>
                {
                    BomItem b;
                    if (!lookup.TryGetValue(bomItem.Id, out b))
                    {
                        lookup.Add(bomItem.Id, bomItem);
                        b = bomItem;
                    }

                    if (mapBomItemToRouteChart == null)
                        return bomItem;

                    if (b.MapBomItemToRouteCharts == null)
                        b.MapBomItemToRouteCharts = new List<MapBomItemToRouteChart>();
                    b.MapBomItemToRouteCharts.Add(mapBomItemToRouteChart);

                    int expensiveCodeLsf82;
                    b.IsExpensive = b.Code_LSF82 != null && expensiveItems.TryGetValue(b.Code_LSF82.Value, out expensiveCodeLsf82);

                    return b;
                }, param, splitOn: "BomItemId", transaction: transaction
            );
            return lookup.Values.ToList();
        }

        private string GetMainSelectQuery(string wherePredicate)
        {
            return $@"SELECT
	                      bi.BomId
	                    , bi.Id
	                    , bi.ParentId
	                    , bip.Detal As ParentDetal
	                    , bip.DetalIma As ParentDetalIma
	                    , bi.Detals
	                    , bi.Detal
	                    , bi.DetalIma
	                    , bi.DetalTyp
	                    , bi.DetalUm
	                    , bi.QtyMnf
	                    , bi.QtyConstr
	                    , bi.QtyRestore
	                    , bi.QtyReplace
	                    , bi.Comment
	                    , bi.Defect
	                    , bi.Decision
	                    , bi.CommentDef
	                    , bi.SerialNumber
	                    , bi.TechnologicalProcessUsed
                        , bi.FinalDecision
                        , bi.IsRequiredSubmit
                        , bi.IsSubmitted
                        , bi.ResearchAction
                        , bi.ResearchResult
	                    , bi.CreateDate
	                    , bi.CreatedBy
                        , ucrt.ActiveDirectoryCN AS CreatedByName
	                    , bi.RecordDate
	                    , bi.UpdatedBy
                        , uupd.ActiveDirectoryCN AS UpdatedByName
                        , bi.IsExpanded
                        , bi.IsShowItem
                        , bi.ClassifierID
                        , bi.ProductID
                        , bi.Code_LSF82
	                    , rm.Name AS RepairMethodName
                        , CASE WHEN EXISTS(SELECT Id FROM BomItem WHERE BomId = bi.BomId and ParentId = bi.Id) THEN 1 ELSE 0 END AS UzelFlag
                        , brc.BomItemId
                        , brc.MkartaId
                        , brc.RouteChart_Number
                        , brc.QtyLaunched
                        , brc.CreateDate
                        , brc.CreatedBy
                    FROM dbo.BomItem bi
                    INNER JOIN dbo.BomHeader bh ON bh.BomId = bi.BomId
                    LEFT JOIN dbo.BomItem bip ON bip.Id = bi.ParentId
                    LEFT JOIN dbo.RepairMethodToItem rmi ON rmi.RootItemId = bh.RootItemId
								  	                    AND rmi.ParentItem = bip.Detal
								  	                    AND rmi.ChildItem = bi.Detal
                    LEFT JOIN dbo.RepairMethod rm ON rm.Id = rmi.RepairMethodId
                    LEFT JOIN dbo.MapBomItemToRouteChart brc ON brc.BomItemId = bi.Id
                    LEFT JOIN dbo.Users ucrt ON ucrt.Login = bi.CreatedBy
                    LEFT JOIN dbo.Users uupd ON uupd.Login = bi.UpdatedBy
                    {wherePredicate}";
        }
    }
}