using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.ExternalData;
using ReporterBusinessLogic;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllDistinctShopEntriesQuery : DbConnectionAsupRepositoryBase, IGetAllDistinctShopEntriesQuery
    {
        public GetAllDistinctShopEntriesQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<Dictionary<int, string>> Execute()
        {
            using (var db = CreateOpenConnection())
            {
                using (var tran = db.BeginReadCommittedRecVersionNoWaitReadWriteTransaction())
                {
                    return await Task.Run(async () =>
                    {
                        var result = (await DbConnection.QueryAsync(Query))
                            .ToDictionary(row => (int)row.PRODUCT_ID, row => (string)row.O_RAST);

                        tran.Commit();

                        return result;
                    });
                }
            }
        }

        private const string Query = @"
            SELECT
                  p.PRODUCT_ID
                , r.O_RAST
            FROM TEHPROC t
            INNER JOIN PRODUCT p ON p.DETALS = t.DETALS
            LEFT JOIN AB_SELECTRASTEHPROC(t.DETALS) r ON 1=1";
    }
}