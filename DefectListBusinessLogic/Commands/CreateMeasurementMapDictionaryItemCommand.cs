using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Exceptions;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    // ─────────────────────────────────────────────────────────────────
    // Создание строки справочника
    // ─────────────────────────────────────────────────────────────────
    public class CreateMeasurementMapDictionaryItemCommand : DbConnectionPmControlRepositoryBase, ICreateMeasurementMapDictionaryItemCommand
    {
        public CreateMeasurementMapDictionaryItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<int> ExecuteAsync(
            MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives,
            string createdBy)
        {
            const string insertItemSql = @"
                INSERT INTO MeasurementMapDictionaryItem
                    (MeasurementMapDictionaryId, CustomNumeration, SortOrder,
                     PossibleDefectId, NominalValueId, AlternateNominalValueId,
                     RecommendedRepairMethodId, RequirementPostRepairId, ItemType,
                     CreateDate, CreatedBy, RecordDate, UpdatedBy)
                VALUES
                    (@DictionaryId, @CustomNumeration, @SortOrder,
                     @PossibleDefectId, @NominalValueId, @AlternateNominalValueId,
                     @RecommendedRepairMethodId, @RequirementPostRepairId, @ItemType,
                     GETDATE(), @CreatedBy, GETDATE(), @UpdatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

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
                    (@ItemId, @DictionaryId, 1,
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
                    var newId = await db.ExecuteScalarAsync<int>(insertItemSql, new
                    {
                        DictionaryId = item.MeasurementMapDictionaryId,
                        item.CustomNumeration,
                        item.SortOrder,
                        item.PossibleDefectId,
                        item.NominalValueId,
                        item.AlternateNominalValueId,
                        item.RecommendedRepairMethodId,
                        item.RequirementPostRepairId,
                        ItemType = (byte)item.ItemType,
                        CreatedBy = createdBy,
                        UpdatedBy = createdBy
                    }, tran);

                    foreach (var method in repairMethodAlternatives)
                    {
                        await db.ExecuteAsync(insertAltSql,
                            new { ItemId = newId, MethodId = method.RecommendedRepairMethodId, CreatedBy = createdBy }, tran);
                    }

                    await db.ExecuteAsync(insertLogSql, new
                    {
                        ItemId = newId,
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
                        RecommendedRepairMethodAlternativeNames = string.Join("; ", repairMethodAlternatives.Select(x => x.RecommendedRepairMethodName).ToList()),
                        item.RequirementPostRepairId,
                        item.RequirementPostRepairName,
                        ItemType = (byte)item.ItemType,
                        CreatedBy = createdBy
                    }, tran);

                    tran.Commit();
                    return newId;
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    tran.Rollback();
                    throw new UniqueConstraintViolationException("Строка с таким возможным дефектом уже существует.");
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