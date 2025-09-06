using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllStateDetalsQuery : DbConnectionPmControlRepositoryBase, IGetAllStateDetalsQuery
    {
        public GetAllStateDetalsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        private readonly string Query =
            "SELECT StateDetalsId AS Id, StateName AS StateDetalsName FROM StateDetals";

        public async Task<IEnumerable<StateDetals>> Execute() =>
                (await DbConnection.QueryAsync<StateDetals>(Query)).ToList();
    }
}