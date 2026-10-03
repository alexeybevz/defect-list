using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    // ─────────────────────────────────────────────────────────────────
    // Создание новой версии справочника (архив старого + копия + Version++)
    // ─────────────────────────────────────────────────────────────────
    public class CreateMeasurementMapDictionaryVersionCommand : DbConnectionPmControlRepositoryBase, ICreateMeasurementMapDictionaryVersionCommand
    {
        private readonly IGetMeasurementMapDictionaryByIdQuery _getMeasurementMapDictionaryByIdQuery;

        public CreateMeasurementMapDictionaryVersionCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetMeasurementMapDictionaryByIdQuery getMeasurementMapDictionaryByIdQuery) : base(dbConnectionFactory)
        {
            _getMeasurementMapDictionaryByIdQuery = getMeasurementMapDictionaryByIdQuery;
        }

        public async Task<int> ExecuteAsync(int sourceDictionaryId, string createdBy)
        {
            const string selectItemsSql = @"
                SELECT
                      i.Id
                    , i.CustomNumeration
                    , i.SortOrder
                    , i.PossibleDefectId
                    , pd.Name AS PossibleDefectName
                    , i.NominalValueId
                    , nv.Name AS NominalValueName
                    , i.AlternateNominalValueId
                    , nv2.Name AS AlternateNominalValueName
                    , i.RecommendedRepairMethodId
                    , rm.Name AS RecommendedRepairMethodName
                    , i.RequirementPostRepairId
                    , pr.Name AS RequirementPostRepairName
                    , i.ItemType
                FROM MeasurementMapDictionaryItem i
                LEFT JOIN PossibleDefect pd               ON pd.Id = i.PossibleDefectId
                LEFT JOIN NominalValue nv                 ON nv.Id = i.NominalValueId
                LEFT JOIN NominalValue nv2                ON nv2.Id = i.AlternateNominalValueId
                LEFT JOIN RecommendedRepairMethod rm      ON rm.Id = i.RecommendedRepairMethodId
                LEFT JOIN RequirementPostRepair pr        ON pr.Id = i.RequirementPostRepairId
                WHERE i.MeasurementMapDictionaryId = @DictionaryId
                ORDER BY i.SortOrder";

            const string selectAltsSql = @"
                SELECT
	                  r.MeasurementMapDictionaryItemId
	                , r.RecommendedRepairMethodId
	                , rm.Name AS RecommendedRepairMethodName
                FROM MeasurementMapDictionaryItemRepairMethodAlternative r
                LEFT JOIN RecommendedRepairMethod rm ON rm.Id = r.RecommendedRepairMethodId
                WHERE r.MeasurementMapDictionaryItemId IN @Ids";

            const string archiveSql = @"
                UPDATE MeasurementMapDictionary
                SET IsActive = 0, RecordDate = GETDATE(), UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            const string insertNewSql = @"
                INSERT INTO MeasurementMapDictionary
                    (Name, Code_LSF82, MeasurementsMapTypeFormId, Version, IsActive,
                     Comment, CreateDate, CreatedBy, RecordDate, UpdatedBy)
                VALUES
                    (@Name, @Code_LSF82, @FormId, @Version, 1,
                     @Comment, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string insertItemSql = @"
                INSERT INTO MeasurementMapDictionaryItem
                    (MeasurementMapDictionaryId, CustomNumeration, SortOrder,
                     PossibleDefectId, NominalValueId, AlternateNominalValueId,
                     RecommendedRepairMethodId, RequirementPostRepairId, ItemType,
                     CreateDate, CreatedBy)
                VALUES
                    (@DictionaryId, @CustomNumeration, @SortOrder,
                     @PossibleDefectId, @NominalValueId, @AlternateNominalValueId,
                     @RecommendedRepairMethodId, @RequirementPostRepairId, @ItemType,
                     GETDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string insertAltSql = @"
                INSERT INTO MeasurementMapDictionaryItemRepairMethodAlternative
                    (MeasurementMapDictionaryItemId, RecommendedRepairMethodId, CreatedBy)
                VALUES (@ItemId, @MethodId, @CreatedBy)";

            const string insertLogSql = @"
                INSERT INTO MeasurementMapDictionaryLog
                    (MeasurementMapDictionaryId, Action, Name, Code_LSF82,
                     MeasurementsMapTypeFormId, Version, IsActive, Comment, BoundRootItems, CreateDate, CreatedBy)
                VALUES (@Id, @Action, @Name, @Code_LSF82, @FormId, @Version, @IsActive, @Comment, @BoundRootItems, GETDATE(), @CreatedBy)";

            const string insertItemLogSql = @"
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
                    (@MeasurementMapDictionaryItemId, @MeasurementMapDictionaryId, 1,
                     @CustomNumeration, @SortOrder,
                     @PossibleDefectId, @PossibleDefectName,
                     @NominalValueId, @NominalValueName,
                     @AlternateNominalValueId, @AlternateNominalValueName,
                     @RecommendedRepairMethodId, @RecommendedRepairMethodName,
                     @RecommendedRepairMethodAlternativeNames,
                     @RequirementPostRepairId, @RequirementPostRepairName,
                     @ItemType, GETDATE(), @CreatedBy)";

            const string deleteBindingsSql = @"DELETE FROM MeasurementMapDictionaryBinding WHERE MeasurementMapDictionaryId = @MeasurementMapDictionaryId";

            const string insertBindingSql = @"
                INSERT INTO MeasurementMapDictionaryBinding
                    (MeasurementMapDictionaryId, RootItemId, Code_LSF82, CreateDate, CreatedBy)
                VALUES
                    (@DictionaryId, @RootItemId, @Code_LSF82, GETDATE(), @CreatedBy)";

            // 1. Читаем источник
            var src = await _getMeasurementMapDictionaryByIdQuery.ExecuteAsync(sourceDictionaryId);

            using (var db = await CreateOpenConnectionAsync())
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    var sourceItems = (await db.QueryAsync<MeasurementMapDictionaryItem>(selectItemsSql,
                        new { DictionaryId = sourceDictionaryId }, tran)).ToList();

                    // 2. Загружаем альтернативы источника (если есть строки)
                    var altsLookup = new Dictionary<int, List<RecommendedRepairMethodAlternative>>();
                    if (sourceItems.Count > 0)
                    {
                        var sourceItemIds = sourceItems.Select(i => i.Id).ToArray();
                        var alts = (await db.QueryAsync(selectAltsSql,
                            new { Ids = sourceItemIds }, tran)).ToList();

                        foreach (var alt in alts)
                        {
                            var dictItemId = (int)alt.MeasurementMapDictionaryItemId;
                            if (!altsLookup.ContainsKey(dictItemId))
                                altsLookup[dictItemId] = new List<RecommendedRepairMethodAlternative>();
                            altsLookup[dictItemId].Add(new RecommendedRepairMethodAlternative()
                            {
                                RecommendedRepairMethodId = alt.RecommendedRepairMethodId,
                                RecommendedRepairMethodName = alt.RecommendedRepairMethodName
                            });
                        }
                    }

                    // 3. Архивируем источник, если не архивирован
                    if (src.IsActive)
                    {
                        await db.ExecuteAsync(archiveSql,
                            new {Id = sourceDictionaryId, UpdatedBy = createdBy}, tran);

                        await db.ExecuteAsync(insertLogSql, new
                        {
                            Id = sourceDictionaryId,
                            Action = 2,
                            Name = src.Name,
                            Code_LSF82 = src.Code_LSF82,
                            FormId = src.MeasurementsMapTypeFormId,
                            Version = src.Version,
                            IsActive = 0,
                            Comment = src.Comment,
                            BoundRootItems = src.BoundRootItemNames,
                            CreatedBy = createdBy
                        }, tran);

                        // 4 Удаляем привязки у справочника-источника
                        await db.ExecuteAsync(deleteBindingsSql, new {MeasurementMapDictionaryId = src.Id}, tran);
                    }

                    // 5. Создаём новую версию
                    var newVersion = src.Version + 1;
                    var newId = await db.ExecuteScalarAsync<int>(insertNewSql, new
                    {
                        Name = src.Name,
                        Code_LSF82 = src.Code_LSF82,
                        FormId = src.MeasurementsMapTypeFormId,
                        Version = newVersion,
                        Comment = src.Comment,
                        CreatedBy = createdBy
                    }, tran);

                    // 6 Вставляем привязки для нового справочника
                    foreach (var bind in src.Bindings)
                    {
                        await db.ExecuteAsync(
                            insertBindingSql,
                            new
                            {
                                DictionaryId = newId,
                                bind.RootItemId,
                                src.Code_LSF82,
                                CreatedBy = createdBy
                            },
                            tran);
                    }

                    await db.ExecuteAsync(insertLogSql, new
                    {
                        Id = newId,
                        Action = 1,
                        Name = src.Name,
                        Code_LSF82 = src.Code_LSF82,
                        FormId = src.MeasurementsMapTypeFormId,
                        Version = newVersion,
                        IsActive = 1,
                        Comment = src.Comment,
                        BoundRootItems = src.BoundRootItemNames,
                        CreatedBy = createdBy
                    }, tran);

                    // 7. Копируем строки
                    foreach (var srcItem in sourceItems)
                    {
                        var srcItemId = srcItem.Id;
                        var newItemId = await db.ExecuteScalarAsync<int>(insertItemSql, new
                        {
                            DictionaryId = newId,
                            CustomNumeration = srcItem.CustomNumeration,
                            SortOrder = srcItem.SortOrder,
                            PossibleDefectId = srcItem.PossibleDefectId,
                            NominalValueId = srcItem.NominalValueId,
                            AlternateNominalValueId = srcItem.AlternateNominalValueId,
                            RecommendedRepairMethodId = srcItem.RecommendedRepairMethodId,
                            RequirementPostRepairId = srcItem.RequirementPostRepairId,
                            ItemType = (byte)srcItem.ItemType,
                            CreatedBy = createdBy
                        }, tran);

                        // 7.1 Копируем альтернативы строки
                        List<RecommendedRepairMethodAlternative> altMethods;
                        if (altsLookup.TryGetValue(srcItemId, out altMethods))
                        {
                            foreach (var altMethod in altMethods)
                            {
                                await db.ExecuteAsync(insertAltSql, new
                                {
                                    ItemId = newItemId,
                                    MethodId = altMethod.RecommendedRepairMethodId,
                                    CreatedBy = createdBy
                                }, tran);
                            }
                        }

                        var recommendedRepairMethodAlternativeNamesa = string.Join("; ",
                            altMethods?
                                .Select(x => x.RecommendedRepairMethodName)
                                .OrderBy(x => x)
                                .ToList() ?? new List<string>());

                        // 7.2 Логируем строки
                        await db.ExecuteAsync(insertItemLogSql, new
                        {
                            MeasurementMapDictionaryItemId = newItemId,
                            MeasurementMapDictionaryId = newId,
                            srcItem.CustomNumeration,
                            srcItem.SortOrder,
                            srcItem.PossibleDefectId,
                            srcItem.PossibleDefectName,
                            srcItem.NominalValueId,
                            srcItem.NominalValueName,
                            srcItem.AlternateNominalValueId,
                            srcItem.AlternateNominalValueName,
                            srcItem.RecommendedRepairMethodId,
                            srcItem.RecommendedRepairMethodName,
                            RecommendedRepairMethodAlternativeNames = recommendedRepairMethodAlternativeNamesa,
                            srcItem.RequirementPostRepairId,
                            srcItem.RequirementPostRepairName,
                            ItemType = (byte)srcItem.ItemType,
                            CreatedBy = createdBy
                        }, tran);
                    }

                    tran.Commit();
                    return newId;
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