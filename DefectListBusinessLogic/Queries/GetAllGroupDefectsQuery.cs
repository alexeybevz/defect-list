using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllGroupDefectsQuery : DbConnectionPmControlRepositoryBase, IGetAllGroupDefectsQuery
    {
        public GetAllGroupDefectsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<GroupDefect>> Execute()
        {
            var query = "SELECT GroupDefectId AS Id, GroupDefectName FROM GroupDefect";

            using (var db = await CreateOpenConnectionAsync())
            {
                return (await db.QueryAsync<GroupDefect>(query)).ToList();
            }
        }
    }
}