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
    public class GetMeasurementMapDictionaryByCodeQuery : DbConnectionPmControlRepositoryBase, IGetMeasurementMapDictionaryByCodeQuery
    {
        public GetMeasurementMapDictionaryByCodeQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<MeasurementMapDictionary> ExecuteAsync(int codeLsf82, int rootItemId)
        {
            // Один запрос: заголовок справочника + все позиции через LEFT JOIN.
            // Multi-mapping Dapper: первый Id — заголовок, второй Id — позиция.
            const string sql = @"
                SELECT
                    d.Id,
                    d.Name,
                    d.Code_LSF82,
                    d.MeasurementsMapTypeFormId,
                    f.Name         AS MeasurementsMapTypeFormName,
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
                    pd.Name        AS PossibleDefectName,
                    i.NominalValueId,
                    nv.Name        AS NominalValueName,
                    i.RecommendedRepairMethodId,
                    rm.Name        AS RecommendedRepairMethodName,
                    i.RequirementPostRepairId,
                    pr.Name        AS RequirementPostRepairName,
                    i.AlternateNominalValueId,
                    nv2.Name       AS AlternateNominalValueName,
                    i.ItemType
                FROM MeasurementMapDictionary d
                -- Привязка к изделию: ищем справочник через Binding по паре (Code_LSF82, RootItemId)
                INNER JOIN MeasurementMapDictionaryBinding b
                    ON  b.MeasurementMapDictionaryId = d.Id
                    AND b.RootItemId                 = @RootItemId
                    AND b.Code_LSF82                 = @Code_LSF82
                INNER JOIN MeasurementsMapTypeForm f ON f.Id = d.MeasurementsMapTypeFormId
                LEFT  JOIN MeasurementMapDictionaryItem i ON i.MeasurementMapDictionaryId = d.Id
                LEFT  JOIN PossibleDefect pd            ON pd.Id = i.PossibleDefectId
                LEFT  JOIN NominalValue nv              ON nv.Id = i.NominalValueId
                LEFT  JOIN NominalValue nv2             ON nv2.Id = i.AlternateNominalValueId
                LEFT  JOIN RecommendedRepairMethod rm   ON rm.Id = i.RecommendedRepairMethodId
                LEFT  JOIN RequirementPostRepair pr     ON pr.Id = i.RequirementPostRepairId
                WHERE d.IsActive = 1
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
                    new { Code_LSF82 = codeLsf82, RootItemId = rootItemId },
                    splitOn: "Id"); // второй Id = начало MeasurementMapDictionaryItem

                if (dictionary == null)
                    return null;

                // Подгружаем альтернативы метода ремонта для всех позиций одним запросом
                await AttachRepairMethodAlternativesAsync(conn, items);

                dictionary.Items = items;

                return dictionary;
            }
        }

        // Один IN-запрос на все позиции справочника, без N+1.
        // Группирует результат по MeasurementMapDictionaryItemId и проставляет
        // каждой строке её набор альтернатив (может быть пустым).
        private async Task AttachRepairMethodAlternativesAsync(
            IDbConnection conn,
            List<MeasurementMapDictionaryItem> items)
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

            var itemIds = items.Select(i => i.Id).ToArray();

            var rows = (await conn.QueryAsync(altSql, new { Ids = itemIds })).Select(x => new
            {
                MeasurementMapDictionaryItemId = (int)x.MeasurementMapDictionaryItemId,
                RecommendedRepairMethodId = (int)x.RecommendedRepairMethodId,
                RecommendedRepairMethodName = (string)x.RecommendedRepairMethodName
            });

            var grouped = rows
                .GroupBy(r => r.MeasurementMapDictionaryItemId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => new RecommendedRepairMethodAlternative
                    {
                        RecommendedRepairMethodId = r.RecommendedRepairMethodId,
                        RecommendedRepairMethodName = r.RecommendedRepairMethodName
                    }).ToList());

            foreach (var item in items)
            {
                List<RecommendedRepairMethodAlternative> alternatives;

                if (grouped.TryGetValue(item.Id, out alternatives))
                    item.RepairMethodAlternatives = alternatives;
            }
        }
    }
}