using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetMeasurementsMapTypeFormsQuery : DbConnectionPmControlRepositoryBase, IGetMeasurementsMapTypeFormsQuery
    {
        public GetMeasurementsMapTypeFormsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IReadOnlyList<MeasurementsMapTypeForm>> ExecuteAsync() =>
            (await DbConnection.QueryAsync<MeasurementsMapTypeForm>("SELECT Id, Name FROM MeasurementsMapTypeForm ORDER BY Id")).ToList();
    }
}