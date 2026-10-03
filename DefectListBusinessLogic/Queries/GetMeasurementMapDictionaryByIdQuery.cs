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
    public class GetMeasurementMapDictionaryByIdQuery : DbConnectionPmControlRepositoryBase, IGetMeasurementMapDictionaryByIdQuery
    {
        public GetMeasurementMapDictionaryByIdQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<MeasurementMapDictionary> ExecuteAsync(int dictionaryId)
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
                    d.UpdatedBy,
                    -- splitOn: второй Id — начало MeasurementMapDictionaryItem
                    i.Id,
                    i.MeasurementMapDictionaryId,
                    i.CustomNumeration,
                    i.SortOrder,
                    i.PossibleDefectId,
                    pd.Name AS PossibleDefectName,
                    i.NominalValueId,
                    nv.Name AS NominalValueName,
                    i.AlternateNominalValueId,
                    nv2.Name AS AlternateNominalValueName,
                    i.RecommendedRepairMethodId,
                    rm.Name AS RecommendedRepairMethodName,
                    i.RequirementPostRepairId,
                    pr.Name AS RequirementPostRepairName,
                    i.ItemType
                FROM MeasurementMapDictionary d
                INNER JOIN MeasurementsMapTypeForm f ON f.Id  = d.MeasurementsMapTypeFormId
                LEFT JOIN MeasurementMapDictionaryItem i  ON i.MeasurementMapDictionaryId = d.Id
                LEFT JOIN PossibleDefect pd               ON pd.Id = i.PossibleDefectId
                LEFT JOIN NominalValue nv                 ON nv.Id = i.NominalValueId
                LEFT JOIN NominalValue nv2                ON nv2.Id = i.AlternateNominalValueId
                LEFT JOIN RecommendedRepairMethod rm      ON rm.Id = i.RecommendedRepairMethodId
                LEFT JOIN RequirementPostRepair pr        ON pr.Id = i.RequirementPostRepairId
                WHERE d.Id = @DictionaryId
                ORDER BY i.SortOrder";

            using (var conn = await CreateOpenConnectionAsync())
            {
                MeasurementMapDictionary dictionary = null;
                var items = new List<MeasurementMapDictionaryItem>();

                await conn.QueryAsync<MeasurementMapDictionary, MeasurementMapDictionaryItem, MeasurementMapDictionary>(
                    sql,
                    (d, item) =>
                    {
                        if (dictionary == null)
                            dictionary = d;
                        if (item != null && item.Id != 0)
                            items.Add(item);
                        return dictionary;
                    },
                    new { DictionaryId = dictionaryId },
                    splitOn: "Id");

                if (dictionary == null)
                    return null;

                await AttachAlternativesAsync(conn, items);
                dictionary.Items = items;

                await AttachBindingsAsync(conn, dictionary);

                return dictionary;
            }
        }

        private async Task AttachAlternativesAsync(IDbConnection conn, List<MeasurementMapDictionaryItem> items)
        {
            if (items.Count == 0)
                return;

            const string altSql = @"
                SELECT
                    a.MeasurementMapDictionaryItemId,
                    a.RecommendedRepairMethodId,
                    rm.Name AS RecommendedRepairMethodName
                FROM MeasurementMapDictionaryItemRepairMethodAlternative a
                INNER JOIN RecommendedRepairMethod rm ON rm.Id = a.RecommendedRepairMethodId
                WHERE a.MeasurementMapDictionaryItemId IN @Ids";

            var ids = items.Select(i => i.Id).ToArray();

            var rows = (await conn.QueryAsync(altSql, new { Ids = ids })).Select(x => new
            {
                MeasurementMapDictionaryItemId = (int)x.MeasurementMapDictionaryItemId,
                RecommendedRepairMethodId = (int)x.RecommendedRepairMethodId,
                RecommendedRepairMethodName = (string)x.RecommendedRepairMethodName
            });

            var grouped = rows
                .GroupBy(r => r.MeasurementMapDictionaryItemId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyCollection<RecommendedRepairMethodAlternative>)g
                        .Select(r => new RecommendedRepairMethodAlternative
                        {
                            RecommendedRepairMethodId = r.RecommendedRepairMethodId,
                            RecommendedRepairMethodName = r.RecommendedRepairMethodName
                        }).ToList());

            foreach (var item in items)
            {
                IReadOnlyCollection<RecommendedRepairMethodAlternative> alternatives;
                if (grouped.TryGetValue(item.Id, out alternatives))
                    item.RepairMethodAlternatives = alternatives;
            }
        }

        // Загружает привязки к изделиям для всех справочников одним запросом (без N+1).
        // По аналогии с AttachRepairMethodAlternativesAsync в других Query-классах.
        // BoundRootItemNames вычисляется автоматически из Bindings в модели MeasurementMapDictionary.
        private async Task AttachBindingsAsync(
            IDbConnection conn,
            MeasurementMapDictionary dictionary)
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
                WHERE b.MeasurementMapDictionaryId = @Id";

            
            var rows = (await conn.QueryAsync<MeasurementMapDictionaryBinding>(
                bindingSql,
                new { Id = dictionary.Id })).ToList();

            var grouped = rows
                .GroupBy(r => r.MeasurementMapDictionaryId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<MeasurementMapDictionaryBinding>)g.ToList());

            IReadOnlyList<MeasurementMapDictionaryBinding> bindings;
            dictionary.Bindings = grouped.TryGetValue(dictionary.Id, out bindings)
                ? bindings.OrderBy(x => x.RootItemName).ToList()
                : new List<MeasurementMapDictionaryBinding>();
        }
    }
}