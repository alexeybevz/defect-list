using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllMapsBomItemToRouteChartsQuery : DbConnectionPmControlRepositoryBase, IGetAllMapsBomItemToRouteChartsQuery
    {
        public GetAllMapsBomItemToRouteChartsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<MapBomItemToRouteChart>> Execute()
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"SELECT
                                  m.BomItemId
                                , m.MkartaId
                                , m.RouteChart_Number
                                , m.QtyLaunched
                                , m.CreateDate
                                , m.CreatedBy
                                , u.ActiveDirectoryCN AS CreatedByName
                                , m.Detal
                              FROM dbo.MapBomItemToRouteChart m
                              LEFT JOIN dbo.Users u ON u.Login = m.CreatedBy";

                return (await db.QueryAsync<MapBomItemToRouteChart>(query)).ToList();
            }
        }

        public async Task<IEnumerable<MapBomItemToRouteChart>> ExecuteByBomItemId(int bomItemId)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"SELECT
                                  m.BomItemId
                                , m.MkartaId
                                , m.RouteChart_Number
                                , m.QtyLaunched
                                , m.CreateDate
                                , m.CreatedBy
                                , u.ActiveDirectoryCN AS CreatedByName
                                , m.Detal
                              FROM dbo.MapBomItemToRouteChart m
                              LEFT JOIN dbo.Users u ON u.Login = m.CreatedBy
                              WHERE BomItemId = @BomItemId";

                return (await db.QueryAsync<MapBomItemToRouteChart>(query, new { BomItemId = bomItemId })).ToList();
            }
        }
    }
}