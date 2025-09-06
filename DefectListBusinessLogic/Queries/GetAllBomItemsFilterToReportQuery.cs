using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllBomItemsFilterToReportQuery : DbConnectionPmControlRepositoryBase, IGetAllBomItemsFilterToReportQuery
    {
        public GetAllBomItemsFilterToReportQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        private const string Query = @"
                SELECT Detal
                FROM dbo.FilterItemsToReport
                WHERE RootItemId = @RootItemId AND ReportName = @ReportName";

        public async Task<IEnumerable<string>> Execute(int rootItemId, string reportName)
        {
            return (await DbConnection.QueryAsync<string>(Query, new {RootItemId = rootItemId, ReportName = reportName})).ToList();
        }
    }
}