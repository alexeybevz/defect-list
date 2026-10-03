using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    // ─────────────────────────────────────────────────────────────────
    // Обновление строки справочника
    // ─────────────────────────────────────────────────────────────────
    public class UpdateMeasurementMapDictionaryItemCommand : DbConnectionPmControlRepositoryBase, IUpdateMeasurementMapDictionaryItemCommand
    {
        public UpdateMeasurementMapDictionaryItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task ExecuteAsync(
            MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives,
            string updatedBy)
        {
            const string updateItemSql = @"
                UPDATE MeasurementMapDictionaryItem SET
                    CustomNumeration       = @CustomNumeration,
                    SortOrder              = @SortOrder,
                    PossibleDefectId       = @PossibleDefectId,
                    NominalValueId         = @NominalValueId,
                    AlternateNominalValueId= @AlternateNominalValueId,
                    RecommendedRepairMethodId = @RecommendedRepairMethodId,
                    RequirementPostRepairId= @RequirementPostRepairId,
                    ItemType               = @ItemType,
                    RecordDate             = GETDATE(),
                    UpdatedBy              = @UpdatedBy
                WHERE Id = @Id";

            // Альтернативы заменяем целиком — проще, чем diff
            const string deleteAltsSql = @"
                DELETE FROM MeasurementMapDictionaryItemRepairMethodAlternative
                WHERE MeasurementMapDictionaryItemId = @ItemId";

            const string insertAltSql = @"
                INSERT INTO MeasurementMapDictionaryItemRepairMethodAlternative
                    (MeasurementMapDictionaryItemId, RecommendedRepairMethodId, CreatedBy)
                VALUES (@ItemId, @MethodId, @CreatedBy)";

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
                    (@ItemId, @DictionaryId, 2,
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
                    await db.ExecuteAsync(updateItemSql, new
                    {
                        item.Id,
                        item.CustomNumeration,
                        item.SortOrder,
                        item.PossibleDefectId,
                        item.NominalValueId,
                        item.AlternateNominalValueId,
                        item.RecommendedRepairMethodId,
                        item.RequirementPostRepairId,
                        ItemType = (byte)item.ItemType,
                        UpdatedBy = updatedBy
                    }, tran);

                    await db.ExecuteAsync(deleteAltsSql, new { ItemId = item.Id }, tran);

                    foreach (var method in repairMethodAlternatives)
                    {
                        await db.ExecuteAsync(insertAltSql,
                            new { ItemId = item.Id, MethodId = method.RecommendedRepairMethodId, CreatedBy = updatedBy }, tran);
                    }

                    var recommendedRepairMethodAlternativeNamesa = string.Join("; ",
                        repairMethodAlternatives?
                            .Select(x => x.RecommendedRepairMethodName)
                            .OrderBy(x => x)
                            .ToList());

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