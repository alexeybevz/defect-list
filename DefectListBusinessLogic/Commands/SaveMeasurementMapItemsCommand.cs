using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Exceptions;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class SaveMeasurementMapItemsCommand : DbConnectionPmControlRepositoryBase, ISaveMeasurementMapItemsCommand
    {
        public SaveMeasurementMapItemsCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task ExecuteAsync(int measurementMapId, IReadOnlyCollection<MeasurementMapItem> items, string updatedBy)
        {
            const string updateItemSql = @"
                UPDATE MeasurementMapItem SET
                    ActualValue = @ActualValue,
                    ActualValueRecordDate = CASE
                        WHEN ActualValue <> @ActualValue OR
                             (ActualValue IS NULL AND @ActualValue IS NOT NULL) OR
                             (ActualValue IS NOT NULL AND @ActualValue IS NULL)
                        THEN GETDATE()
                        ELSE ActualValueRecordDate
                    END,
                    ActualRecommendedRepairMethodId = @ActualRecommendedRepairMethodId,
                    MarkOfWorkCompletion = @MarkOfWorkCompletion,
                    MarkOfWorkCompletionBy = @MarkOfWorkCompletionBy,
                    RecordDate = GETDATE(),
                    UpdatedBy = @UpdatedBy,
                    RowVersion = RowVersion + 1
                OUTPUT INSERTED.RowVersion
                WHERE Id = @Id
                  AND RowVersion = @ExpectedRowVersion";

            const string insertItemLogSql = @"
                INSERT INTO MeasurementMapItemLog
                    (MeasurementMapItemId, MeasurementMapId, Action,
                     CustomNumeration, SortOrder,
                     PossibleDefectId, PossibleDefectName,
                     NominalValueId, NominalValueName,
                     AlternateNominalValueId, AlternateNominalValueName, ItemType,
                     RecommendedRepairMethodId, RecommendedRepairMethodName,
                     RequirementPostRepairId, RequirementPostRepairName, ActualValue,
                     ActualRecommendedRepairMethodId, ActualRecommendedRepairMethodName,
                     MarkOfWorkCompletion, MarkOfWorkCompletionBy,
                     CreateDate, CreatedBy)
                VALUES
                    (@ItemId, @MapId, 2,
                     @CustomNumeration, @SortOrder,
                     @PossibleDefectId, @PossibleDefectName,
                     @NominalValueId, @NominalValueName,
                     @AlternateNominalValueId, @AlternateNominalValueName, @ItemType,
                     @RecommendedRepairMethodId, @RecommendedRepairMethodName,
                     @RequirementPostRepairId, @RequirementPostRepairName, @ActualValue,
                     @ActualRecommendedRepairMethodId, @ActualRecommendedRepairMethodName,
                     @MarkOfWorkCompletion, @MarkOfWorkCompletionBy,
                     GETDATE(), @UpdatedBy)";

            using (var db = await CreateOpenConnectionAsync())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            var newRowVersion = await db.ExecuteScalarAsync<int?>(
                                updateItemSql,
                                new
                                {
                                    item.Id,
                                    item.ActualValue,
                                    item.ActualRecommendedRepairMethodId,
                                    item.MarkOfWorkCompletion,
                                    item.MarkOfWorkCompletionBy,
                                    UpdatedBy = updatedBy,
                                    ExpectedRowVersion = item.RowVersion
                                },
                                tran);

                            if (newRowVersion == null)
                                throw new ConcurrencyConflictException($"Строка карты измерения (№ {item.CustomNumeration}) была изменена другим пользователем. " +
                                                                       "Форма карты измерения будет обновлена для загрузки актуальных данных. После этого внесите требуемые изменения.");

                            await db.ExecuteAsync(
                                insertItemLogSql,
                                new
                                {
                                    ItemId = item.Id,
                                    MapId = measurementMapId,
                                    item.CustomNumeration,
                                    item.SortOrder,
                                    item.PossibleDefectId,
                                    item.PossibleDefectName,
                                    item.NominalValueId,
                                    item.NominalValueName,
                                    item.AlternateNominalValueId,
                                    item.AlternateNominalValueName,
                                    item.ItemType,
                                    item.RecommendedRepairMethodId,
                                    item.RecommendedRepairMethodName,
                                    item.RequirementPostRepairId,
                                    item.RequirementPostRepairName,
                                    item.ActualValue,
                                    item.ActualRecommendedRepairMethodId,
                                    item.ActualRecommendedRepairMethodName,
                                    item.MarkOfWorkCompletion,
                                    item.MarkOfWorkCompletionBy,
                                    UpdatedBy = updatedBy
                                },
                                tran);
                        }

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
}