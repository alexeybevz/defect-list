using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomHeaderSubscribersQuery : DbConnectionPmControlRepositoryBase, IGetAllBomHeaderSubscribersQuery
    {
        public GetAllBomHeaderSubscribersQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        const string Query = @"SELECT UserId, BomId FROM BomHeaderSubscribers";

        public async Task<IEnumerable<BomHeaderSubscriber>> Execute() => 
                (await DbConnection.QueryAsync<BomHeaderSubscriber>(Query)).ToList();
    }
}