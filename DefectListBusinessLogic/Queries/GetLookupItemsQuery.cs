using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetLookupItemsQuery : DbConnectionPmControlRepositoryBase, IGetLookupItemsQuery
    {
        public GetLookupItemsQuery(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory) { }

        public async Task<IReadOnlyList<LookupItem>> ExecuteAsync(LookupItemKind kind)
        {
            var tableName = GetTableName(kind);
            var usageSql = GetUsageSql(kind);

            // Загружаем все записи (включая неактивные)
            var itemsSql = $"SELECT Id, Name, IsActive FROM {tableName} ORDER BY Name";

            using (var db = await CreateOpenConnectionAsync())
            {
                var items = (await db.QueryAsync<LookupItem>(itemsSql)).ToList();

                if (items.Count == 0)
                    return items;

                // Определяем, какие записи используются в строках справочника или экземплярах карт
                var usedIds = new HashSet<int>(await db.QueryAsync<int>(usageSql));

                foreach (var item in items)
                    item.IsUsed = usedIds.Contains(item.Id);

                return items;
            }
        }

        private static string GetTableName(LookupItemKind kind)
        {
            switch (kind)
            {
                case LookupItemKind.PossibleDefect: return "PossibleDefect";
                case LookupItemKind.NominalValue: return "NominalValue";
                case LookupItemKind.RecommendedRepairMethod: return "RecommendedRepairMethod";
                case LookupItemKind.RequirementPostRepair: return "RequirementPostRepair";
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        // Возвращает SQL, возвращающий Id записей справочника, которые
        // используются хотя бы в одной строке словаря ИЛИ в экземпляре карты.
        // Используем UNION чтобы покрыть оба контекста одним запросом.
        private static string GetUsageSql(LookupItemKind kind)
        {
            switch (kind)
            {
                case LookupItemKind.PossibleDefect:
                    return @"
                        SELECT DISTINCT PossibleDefectId FROM MeasurementMapDictionaryItem WHERE PossibleDefectId IS NOT NULL
                        UNION
                        SELECT DISTINCT PossibleDefectId FROM MeasurementMapItem WHERE PossibleDefectId IS NOT NULL";

                case LookupItemKind.NominalValue:
                    return @"
                        SELECT DISTINCT NominalValueId      FROM MeasurementMapDictionaryItem WHERE NominalValueId IS NOT NULL
                        UNION
                        SELECT DISTINCT AlternateNominalValueId FROM MeasurementMapDictionaryItem WHERE AlternateNominalValueId IS NOT NULL
                        UNION
                        SELECT DISTINCT NominalValueId      FROM MeasurementMapItem WHERE NominalValueId IS NOT NULL
                        UNION
                        SELECT DISTINCT AlternateNominalValueId FROM MeasurementMapItem WHERE AlternateNominalValueId IS NOT NULL";

                case LookupItemKind.RecommendedRepairMethod:
                    return @"
                        SELECT DISTINCT RecommendedRepairMethodId FROM MeasurementMapDictionaryItem WHERE RecommendedRepairMethodId IS NOT NULL
                        UNION
                        SELECT DISTINCT RecommendedRepairMethodId FROM MeasurementMapDictionaryItemRepairMethodAlternative
                        UNION
                        SELECT DISTINCT RecommendedRepairMethodId FROM MeasurementMapItem WHERE RecommendedRepairMethodId IS NOT NULL
                        UNION
                        SELECT DISTINCT ActualRecommendedRepairMethodId FROM MeasurementMapItem WHERE ActualRecommendedRepairMethodId IS NOT NULL";

                case LookupItemKind.RequirementPostRepair:
                    return @"
                        SELECT DISTINCT RequirementPostRepairId FROM MeasurementMapDictionaryItem WHERE RequirementPostRepairId IS NOT NULL
                        UNION
                        SELECT DISTINCT RequirementPostRepairId FROM MeasurementMapItem WHERE RequirementPostRepairId IS NOT NULL";

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }
}