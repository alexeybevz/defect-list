using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Queries;
using Dapper;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomItemsWithSerialNumbersQuery : DbConnectionPmControlRepositoryBase, IGetAllBomItemsWithSerialNumbersQuery
    {
        public GetAllBomItemsWithSerialNumbersQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<Dictionary<string, IEnumerable<int>>> Execute()
        {
            var query = @"SELECT
                              bi.SerialNumber
	                        , bi.BomId
                          FROM dbo.BomItem bi
                          WHERE bi.SerialNumber IS NOT NULL
                          ORDER BY bi.SerialNumber";

            var serialEntries = new List<SerialEntry>();
            using (var db = await CreateOpenConnectionAsync())
            {
                serialEntries = (await db.QueryAsync<SerialEntry>(query)).ToList();
            }

            var dictionary = new Dictionary<string, HashSet<int>>();
            foreach (var serialEntry in serialEntries)
            {
                HashSet<int> bomIds;
                if (!dictionary.TryGetValue(serialEntry.SerialNumber, out bomIds))
                {
                    bomIds = new HashSet<int>();
                    dictionary[serialEntry.SerialNumber] = bomIds;
                }
                bomIds.Add(serialEntry.BomId);
            }

            return dictionary.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.AsEnumerable());
        }
    }
}