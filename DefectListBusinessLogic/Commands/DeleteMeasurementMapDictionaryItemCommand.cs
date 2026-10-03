using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    // ─────────────────────────────────────────────────────────────────
    // Удаление строки справочника (физическое + лог)
    // ─────────────────────────────────────────────────────────────────
    public class DeleteMeasurementMapDictionaryItemCommand : DbConnectionPmControlRepositoryBase, IDeleteMeasurementMapDictionaryItemCommand
    {
        public DeleteMeasurementMapDictionaryItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task ExecuteAsync(MeasurementMapDictionaryItem item, string updatedBy)
        {
            const string checkUsageSql = @"
                SELECT COUNT(1) FROM MeasurementMapItem
                WHERE MeasurementMapDictionaryItemId = @Id";

            const string selectAltsSql = @"
                SELECT
	                  r.MeasurementMapDictionaryItemId
	                , r.RecommendedRepairMethodId
	                , rm.Name AS RecommendedRepairMethodName
                FROM MeasurementMapDictionaryItemRepairMethodAlternative r
                LEFT JOIN RecommendedRepairMethod rm ON rm.Id = r.RecommendedRepairMethodId
                WHERE r.MeasurementMapDictionaryItemId = @Id";

            const string deleteAltsSql = @"
                DELETE FROM MeasurementMapDictionaryItemRepairMethodAlternative
                WHERE MeasurementMapDictionaryItemId = @Id";

            const string deleteItemSql = @"
                DELETE FROM MeasurementMapDictionaryItem WHERE Id = @Id";

            const string insertLogSql = @"
                INSERT INTO MeasurementMapDictionaryItemLog
                    (MeasurementMapDictionaryItemId, MeasurementMapDictionaryId, Action,
                     CustomNumeration, SortOrder,
                     PossibleDefectId, PossibleDefectName,
                     NominalValueId, NominalValueName,
                     AlternateNominalValueId, AlternateNominalValueName,
                     RecommendedRepairMethodId, RecommendedRepairMethodName,
                     RecommendedRepairMethodAlternativeNames,
                     RequirementPostRepairId, RequirementPostRepairName,
                     ItemType, CreateDate, CreatedBy)
                VALUES
                    (@ItemId, @DictionaryId, 3,
                     @CustomNumeration, @SortOrder,
                     @PossibleDefectId, @PossibleDefectName,
                     @NominalValueId, @NominalValueName,
                     @AlternateNominalValueId, @AlternateNominalValueName,
                     @RecommendedRepairMethodId, @RecommendedRepairMethodName,
                     @RecommendedRepairMethodAlternativeNames,
                     @RequirementPostRepairId, @RequirementPostRepairName,
                     @ItemType, GETDATE(), @CreatedBy)";

            using (var db = await CreateOpenConnectionAsync())
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    var usageCount = await db.ExecuteScalarAsync<int>(
                        checkUsageSql, new { item.Id }, tran);

                    if (usageCount > 0)
                        throw new System.InvalidOperationException(
                            $"Строка справочника (№ {item.CustomNumeration}) используется в {usageCount} экземплярах карт измерений. " +
                            "Удаление невозможно.");

                    // Загружаем альтернативы источника (если есть строки)
                    var altMethods = (await db.QueryAsync<RecommendedRepairMethodAlternative>(selectAltsSql,
                        new { Id = item.Id }, tran)).ToList();

                    var recommendedRepairMethodAlternativeNamesa = string.Join("; ",
                        altMethods?
                            .Select(x => x.RecommendedRepairMethodName)
                            .OrderBy(x => x)
                            .ToList());

                    // Пишем лог до удаления — после уже не достать данные
                    await db.ExecuteAsync(insertLogSql, new
                    {
                        ItemId = item.Id,
                        DictionaryId = item.MeasurementMapDictionaryId,
                        item.CustomNumeration,
                        item.SortOrder,
                        item.PossibleDefectId,
                        item.PossibleDefectName,
                        item.NominalValueId,
                        item.NominalValueName,
                        item.AlternateNominalValueId,
                        item.AlternateNominalValueName,
                        item.RecommendedRepairMethodId,
                        item.RecommendedRepairMethodName,
                        RecommendedRepairMethodAlternativeNames = recommendedRepairMethodAlternativeNamesa,
                        item.RequirementPostRepairId,
                        item.RequirementPostRepairName,
                        ItemType = (byte)item.ItemType,
                        CreatedBy = updatedBy
                    }, tran);

                    await db.ExecuteAsync(deleteAltsSql, new { item.Id }, tran);
                    await db.ExecuteAsync(deleteItemSql, new { item.Id }, tran);

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}