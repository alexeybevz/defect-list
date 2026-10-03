using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Exceptions;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class CreateMeasurementMapCommand : DbConnectionPmControlRepositoryBase, ICreateMeasurementMapCommand
    {
        private readonly IGetMeasurementMapDictionaryByCodeQuery _getMeasurementMapDictionaryByCodeQuery;

        public CreateMeasurementMapCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetMeasurementMapDictionaryByCodeQuery getMeasurementMapDictionaryByCodeQuery) : base(dbConnectionFactory)
        {
            _getMeasurementMapDictionaryByCodeQuery = getMeasurementMapDictionaryByCodeQuery;
        }

        public async Task<MeasurementMap> ExecuteAsync(int bomItemId, int codeLsf82, int rootItemId, string createdBy)
        {
            var measurementMapDictionary = await _getMeasurementMapDictionaryByCodeQuery.ExecuteAsync(codeLsf82, rootItemId);

            if (measurementMapDictionary == null)
                throw new InvalidOperationException(
                    $"Справочник карты измерений для номенклатуры не найден (Code_LSF82={codeLsf82}, RootItemId={rootItemId}). " +
                    "Обратитесь к администратору для настройки справочника.");

            using (var db = await CreateOpenConnectionAsync())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        // 1. Создаём заголовок MeasurementMap
                        const string insertMapSql = @"
                            INSERT INTO MeasurementMap
                                (BomItemId, MeasurementMapDictionaryId, MeasurementsMapTypeFormId,
                                 Status, CreateDate, CreatedBy, RecordDate, UpdatedBy)
                            VALUES
                                (@BomItemId, @DictionaryId, @FormId,
                                 1, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
                            SELECT CAST(SCOPE_IDENTITY() AS int);";

                        var mapId = await db.ExecuteScalarAsync<int>(
                            insertMapSql,
                            new
                            {
                                BomItemId = bomItemId,
                                DictionaryId = measurementMapDictionary.Id,
                                FormId = measurementMapDictionary.MeasurementsMapTypeFormId,
                                CreatedBy = createdBy
                            },
                            tran);

                        // 2. Копируем позиции из справочника
                        const string insertItemSql = @"
                            INSERT INTO MeasurementMapItem
                                (MeasurementMapId, MeasurementMapDictionaryItemId,
                                 CustomNumeration, SortOrder,
                                 PossibleDefectId, NominalValueId,
                                 AlternateNominalValueId, ItemType,
                                 RecommendedRepairMethodId, RequirementPostRepairId,
                                 ActualRecommendedRepairMethodId,
                                 CreateDate, CreatedBy, RecordDate, UpdatedBy)
                            VALUES
                                (@MapId, @DictionaryItemId,
                                 @CustomNumeration, @SortOrder,
                                 @PossibleDefectId, @NominalValueId,
                                 @AlternateNominalValueId, @ItemType,
                                 @RecommendedRepairMethodId, @RequirementPostRepairId,
                                 @ActualRecommendedRepairMethodId,
                                 GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
                            SELECT CAST(SCOPE_IDENTITY() AS int);";

                        var createdItems = new List<MeasurementMapItem>();

                        foreach (var di in measurementMapDictionary.Items)
                        {
                            var itemId = await db.ExecuteScalarAsync<int>(
                                insertItemSql,
                                new
                                {
                                    MapId = mapId,
                                    DictionaryItemId = di.Id,
                                    di.CustomNumeration,
                                    di.SortOrder,
                                    di.PossibleDefectId,
                                    di.NominalValueId,
                                    di.AlternateNominalValueId,
                                    di.ItemType,
                                    di.RecommendedRepairMethodId,
                                    di.RequirementPostRepairId,
                                    ActualRecommendedRepairMethodId = di.ItemType == MeasurementMapItemType.Numeric
                                        ? null
                                        : di.RecommendedRepairMethodId,
                                    CreatedBy = createdBy
                                },
                                tran);

                            createdItems.Add(new MeasurementMapItem
                            {
                                Id = itemId,
                                MeasurementMapId = mapId,
                                MeasurementMapDictionaryItemId = di.Id,
                                CustomNumeration = di.CustomNumeration,
                                SortOrder = di.SortOrder,
                                PossibleDefectId = di.PossibleDefectId,
                                PossibleDefectName = di.PossibleDefectName,
                                NominalValueId = di.NominalValueId,
                                NominalValueName = di.NominalValueName,
                                AlternateNominalValueId = di.AlternateNominalValueId,
                                AlternateNominalValueName = di.AlternateNominalValueName,
                                ItemType = di.ItemType,
                                RecommendedRepairMethodId = di.RecommendedRepairMethodId,
                                RecommendedRepairMethodName = di.RecommendedRepairMethodName,
                                RequirementPostRepairId = di.RequirementPostRepairId,
                                RequirementPostRepairName = di.RequirementPostRepairName,
                                ActualRecommendedRepairMethodId = di.ItemType == MeasurementMapItemType.Numeric
                                    ? null
                                    : di.RecommendedRepairMethodId,
                                ActualRecommendedRepairMethodName = di.ItemType == MeasurementMapItemType.Numeric
                                    ? null
                                    : di.RecommendedRepairMethodName,
                                // RepairMethodAlternatives не проставляем здесь —
                                // при следующей загрузке через GetMeasurementMapByBomItemIdQuery
                                // они подтянутся from MeasurementMapDictionaryItemId.
                            });
                        }

                        // 3. Лог создания заголовка
                        const string insertMapLogSql = @"
                            INSERT INTO MeasurementMapLog
                                (MeasurementMapId, Action, BomItemId,
                                 MeasurementMapDictionaryId, MeasurementsMapTypeFormId,
                                 Status, Comment, CreateDate, CreatedBy)
                            VALUES
                                (@MapId, 1, @BomItemId,
                                 @DictionaryId, @FormId,
                                 1, NULL, GETDATE(), @CreatedBy)";

                        await db.ExecuteAsync(
                            insertMapLogSql,
                            new
                            {
                                MapId = mapId,
                                BomItemId = bomItemId,
                                DictionaryId = measurementMapDictionary.Id,
                                FormId = measurementMapDictionary.MeasurementsMapTypeFormId,
                                CreatedBy = createdBy
                            },
                            tran);

                        // 4. Создание лога items
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
                                 CreateDate, CreatedBy)
                            VALUES
                                (@MeasurementMapItemId, @MeasurementMapId, @Action,
                                 @CustomNumeration, @SortOrder,
                                 @PossibleDefectId, @PossibleDefectName,
                                 @NominalValueId, @NominalValueName,
                                 @AlternateNominalValueId, @AlternateNominalValueName, @ItemType,
                                 @RecommendedRepairMethodId, @RecommendedRepairMethodName,
                                 @RequirementPostRepairId, @RequirementPostRepairName, @ActualValue,
                                 @ActualRecommendedRepairMethodId, @ActualRecommendedRepairMethodName,
                                 GETDATE(), @CreatedBy);
                            SELECT CAST(SCOPE_IDENTITY() AS int);";

                        foreach (var item in createdItems)
                        {
                            await db.ExecuteAsync(insertItemLogSql, new
                            {
                                MeasurementMapItemId = item.Id,
                                MeasurementMapId = item.MeasurementMapId,
                                Action = 1,
                                CustomNumeration = item.CustomNumeration,
                                SortOrder = item.SortOrder,
                                PossibleDefectId = item.PossibleDefectId,
                                PossibleDefectName = item.PossibleDefectName,
                                NominalValueId = item.NominalValueId,
                                NominalValueName = item.NominalValueName,
                                AlternateNominalValueId = item.AlternateNominalValueId,
                                AlternateNominalValueName = item.AlternateNominalValueName,
                                ItemType = item.ItemType,
                                RecommendedRepairMethodId = item.RecommendedRepairMethodId,
                                RecommendedRepairMethodName = item.RecommendedRepairMethodName,
                                RequirementPostRepairId = item.RequirementPostRepairId,
                                RequirementPostRepairName = item.RequirementPostRepairName,
                                ActualValue = item.ActualValue,
                                ActualRecommendedRepairMethodId = item.ActualRecommendedRepairMethodId,
                                ActualRecommendedRepairMethodName = item.ActualRecommendedRepairMethodName,
                                CreatedBy = createdBy
                            }, tran);
                        }

                        tran.Commit();

                        return new MeasurementMap
                        {
                            Id = mapId,
                            BomItemId = bomItemId,
                            MeasurementMapDictionaryId = measurementMapDictionary.Id,
                            MeasurementsMapTypeFormId = measurementMapDictionary.MeasurementsMapTypeFormId,
                            Status = 1,
                            Items = createdItems
                        };
                    }
                    catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                    {
                        tran.Rollback();
                        throw new MeasurementMapAlreadyExistsException(
                            "Карта измерения для данной ДСЕ уже была создана другим пользователем. " + 
                            "Форма будет обновлена для загрузки актуальных данных.");
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