using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllMeasurementMapDictionariesQuery : DbConnectionPmControlRepositoryBase, IGetAllMeasurementMapDictionariesQuery
    {
        public GetAllMeasurementMapDictionariesQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IReadOnlyList<MeasurementMapDictionary>> ExecuteAsync()
        {
            const string sql = @"
                SELECT
                    d.Id,
                    d.Name,
                    d.Code_LSF82,
                    d.MeasurementsMapTypeFormId,
                    f.Name AS MeasurementsMapTypeFormName,
                    d.Version,
                    d.IsActive,
                    d.Comment,
                    d.CreateDate,
                    d.CreatedBy,
                    d.RecordDate,
                    d.UpdatedBy
                FROM MeasurementMapDictionary d
                INNER JOIN MeasurementsMapTypeForm f ON f.Id = d.MeasurementsMapTypeFormId
                ORDER BY d.Code_LSF82, d.Version DESC";

            using (var conn = await CreateOpenConnectionAsync())
            {
                var dictionaries = (await conn.QueryAsync<MeasurementMapDictionary>(sql)).ToList();

                if (dictionaries.Count == 0)
                    return dictionaries;

                await AttachBindingsAsync(conn, dictionaries);

                return dictionaries;
            }
        }

        // Загружает привязки к изделиям для всех справочников одним запросом (без N+1).
        // По аналогии с AttachRepairMethodAlternativesAsync в других Query-классах.
        // BoundRootItemNames вычисляется автоматически из Bindings в модели MeasurementMapDictionary.
        private async Task AttachBindingsAsync(
            IDbConnection conn,
            List<MeasurementMapDictionary> dictionaries)
        {
            const string bindingSql = @"
                SELECT
                    b.MeasurementMapDictionaryId,
                    b.Id,
                    b.RootItemId,
                    b.Code_LSF82,
                    r.Izdel AS RootItemName
                FROM MeasurementMapDictionaryBinding b
                INNER JOIN RootItem r ON r.Id = b.RootItemId
                WHERE b.MeasurementMapDictionaryId IN @Ids";

            var dictionaryIds = dictionaries.Select(d => d.Id).ToArray();

            var rows = (await conn.QueryAsync<MeasurementMapDictionaryBinding>(
                bindingSql,
                new { Ids = dictionaryIds })).ToList();

            var grouped = rows
                .GroupBy(r => r.MeasurementMapDictionaryId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<MeasurementMapDictionaryBinding>)g.ToList());

            foreach (var dictionary in dictionaries)
            {
                IReadOnlyList<MeasurementMapDictionaryBinding> bindings;
                dictionary.Bindings = grouped.TryGetValue(dictionary.Id, out bindings)
                    ? bindings.OrderBy(x => x.RootItemName).ToList()
                    : new List<MeasurementMapDictionaryBinding>();
            }
        }
    }
}