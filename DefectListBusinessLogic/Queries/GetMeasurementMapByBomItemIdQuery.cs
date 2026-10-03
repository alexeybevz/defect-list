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
    public class GetMeasurementMapByBomItemIdQuery : DbConnectionPmControlRepositoryBase, IGetMeasurementMapByBomItemIdQuery
    {
        public GetMeasurementMapByBomItemIdQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<MeasurementMap> ExecuteAsync(int bomItemId)
        {
            const string sql = @"
                SELECT
                    m.Id,
                    m.BomItemId,
                    m.MeasurementMapDictionaryId,
                    m.MeasurementsMapTypeFormId,
                    f.Name         AS MeasurementsMapTypeFormName,
                    m.Status,
                    m.Comment,
                    m.CreateDate,
                    m.CreatedBy,
                    m.RecordDate,
                    m.UpdatedBy,
                    -- splitOn: второй Id — начало MeasurementMapItem
                    i.Id,
                    i.MeasurementMapId,
                    i.MeasurementMapDictionaryItemId,
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
                    i.ActualValue,
                    i.ActualValueRecordDate,
                    i.ActualRecommendedRepairMethodId,
                    rm2.Name        AS ActualRecommendedRepairMethodName,
                    i.MarkOfWorkCompletion,
                    i.MarkOfWorkCompletionBy,
                    i.CreateDate,
                    i.CreatedBy,
                    ucrt.ActiveDirectoryCN AS CreatedByName,
                    i.RecordDate,
                    i.UpdatedBy,
                    uupd.ActiveDirectoryCN AS UpdatedByName,
                    i.AlternateNominalValueId,
                    nv2.Name       AS AlternateNominalValueName,
                    i.ItemType,
                    i.RowVersion
                FROM MeasurementMap m
                INNER JOIN MeasurementsMapTypeForm f ON f.Id = m.MeasurementsMapTypeFormId
                INNER JOIN MeasurementMapDictionary md ON md.Id = m.MeasurementMapDictionaryId
                LEFT  JOIN MeasurementMapItem i      ON i.MeasurementMapId = m.Id
                LEFT  JOIN PossibleDefect pd         ON pd.Id = i.PossibleDefectId
                LEFT  JOIN NominalValue nv           ON nv.Id = i.NominalValueId
                LEFT  JOIN NominalValue nv2          ON nv2.Id = i.AlternateNominalValueId
                LEFT  JOIN RecommendedRepairMethod rm ON rm.Id = i.RecommendedRepairMethodId
                LEFT  JOIN RecommendedRepairMethod rm2 ON rm2.Id = i.ActualRecommendedRepairMethodId
                LEFT  JOIN RequirementPostRepair pr   ON pr.Id = i.RequirementPostRepairId
                LEFT  JOIN Users ucrt                 ON ucrt.[Login] = i.CreatedBy
                LEFT  JOIN Users uupd                 ON uupd.[Login] = i.UpdatedBy
                WHERE m.BomItemId = @BomItemId
                ORDER BY i.SortOrder";

            using (var db = await CreateOpenConnectionAsync())
            {
                MeasurementMap map = null;
                var items = new List<MeasurementMapItem>();

                await db.QueryAsync<MeasurementMap, MeasurementMapItem, MeasurementMap>(
                    sql,
                    (m, item) =>
                    {
                        if (map == null)
                            map = m;

                        if (item != null && item.Id != 0)
                            items.Add(item);

                        return map;
                    },
                    new { BomItemId = bomItemId },
                    splitOn: "Id");

                if (map == null)
                    return null;

                await AttachRepairMethodAlternativesAsync(db, items);

                map.SketchFilePaths = new List<string>();
                await AttachSketchFilesAsynC(db, map);

                map.Items = items;

                return map;
            }
        }

        private async Task AttachSketchFilesAsynC(IDbConnection db, MeasurementMap map)
        {
            if (map == null)
                return;

            const string sql = @"
                SELECT SketchFilePath
                FROM MeasurementMapDictionarySketchFile
                WHERE MeasurementMapDictionaryId = @MeasurementMapDictionaryId";

            var sketchs = (await db.QueryAsync<string>(sql, new {map.MeasurementMapDictionaryId})).ToList();

            if (sketchs.Any())
                map.SketchFilePaths.AddRange(sketchs);
        }

        // Альтернативы привязаны к MeasurementMapDictionaryItemId (строке справочника),
        // поэтому для строк карты, добавленных вручную без привязки к справочнику
        // (MeasurementMapDictionaryItemId == null), альтернатив не будет —
        // ячейка останется readonly, что корректно: для произвольной строки
        // нет заранее заданного набора допустимых методов.
        private async Task AttachRepairMethodAlternativesAsync(
            IDbConnection conn,
            List<MeasurementMapItem> items)
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

            var dictItemIds = items
                .Where(i => i.MeasurementMapDictionaryItemId.HasValue)
                .Select(i => i.MeasurementMapDictionaryItemId.Value)
                .Distinct()
                .ToArray();

            if (dictItemIds.Length == 0)
                return;

            var rows = (await conn.QueryAsync(altSql, new { Ids = dictItemIds })).Select(x => new
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

                if (item.MeasurementMapDictionaryItemId.HasValue &&
                    grouped.TryGetValue(item.MeasurementMapDictionaryItemId.Value, out alternatives))
                    item.RepairMethodAlternatives = alternatives;
            }
        }
    }
}