using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllRootItemsQuery : DbConnectionPmControlRepositoryBase, IGetAllRootItemsQuery
    {
        public GetAllRootItemsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        private readonly string Query = @"
                        SELECT
                             Id
                           , Izdels
                           , Izdel
                           , IzdelInitial
                           , IzdelIma
                           , IzdelTyp
                        FROM RootItem";

        public async Task<IEnumerable<RootItem>> Execute() =>
                (await DbConnection.QueryAsync<RootItem>(Query)).ToList();
    }
}