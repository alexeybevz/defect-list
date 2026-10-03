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
    // Создание нового справочника (шапка, без строк)
    // ─────────────────────────────────────────────────────────────────
    public class CreateMeasurementMapDictionaryCommand
        : DbConnectionPmControlRepositoryBase, ICreateMeasurementMapDictionaryCommand
    {
        public CreateMeasurementMapDictionaryCommand(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory) { }

        public async Task<MeasurementMapDictionary> ExecuteAsync(MeasurementMapDictionary dictionary, IReadOnlyList<RootItem> rootItems, string createdBy)
        {
            const string insertSql = @"
                INSERT INTO MeasurementMapDictionary
                    (Name, Code_LSF82, MeasurementsMapTypeFormId, Version, IsActive,
                     Comment, CreateDate, CreatedBy, RecordDate, UpdatedBy)
                VALUES
                    (@Name, @Code_LSF82, @FormId, 1, 1,
                     @Comment, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string logSql = @"
                INSERT INTO MeasurementMapDictionaryLog
                    (MeasurementMapDictionaryId, Action, Name, Code_LSF82,
                     MeasurementsMapTypeFormId, Version, IsActive, Comment, BoundRootItems, CreateDate, CreatedBy)
                VALUES
                    (@Id, 1, @Name, @Code_LSF82, @FormId, 1, 1, @Comment, @BoundRootItems, GETDATE(), @CreatedBy)";

            const string insertBindingSql = @"
                INSERT INTO MeasurementMapDictionaryBinding
                    (MeasurementMapDictionaryId, RootItemId, Code_LSF82, CreateDate, CreatedBy)
                VALUES
                    (@DictionaryId, @RootItemId, @Code_LSF82, GETDATE(), @CreatedBy)";

            using (var db = await CreateOpenConnectionAsync())
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    var id = await db.ExecuteScalarAsync<int>(insertSql, new
                    {
                        dictionary.Name,
                        dictionary.Code_LSF82,
                        FormId = dictionary.MeasurementsMapTypeFormId,
                        dictionary.Comment,
                        CreatedBy = createdBy
                    }, tran);

                    foreach (var rootItem in rootItems)
                    {
                        await db.ExecuteAsync(
                            insertBindingSql,
                            new
                            {
                                DictionaryId = id,
                                RootItemId = rootItem.Id,
                                dictionary.Code_LSF82,
                                CreatedBy = createdBy
                            },
                            tran);
                    }

                    await db.ExecuteAsync(logSql, new
                    {
                        Id = id,
                        dictionary.Name,
                        dictionary.Code_LSF82,
                        FormId = dictionary.MeasurementsMapTypeFormId,
                        dictionary.Comment,
                        BoundRootItems = string.Join("; ", rootItems.Select(x => x.Izdel).ToList()),
                        CreatedBy = createdBy
                    }, tran);

                    tran.Commit();

                    return new MeasurementMapDictionary
                    {
                        Id = id,
                        Name = dictionary.Name,
                        Code_LSF82 = dictionary.Code_LSF82,
                        MeasurementsMapTypeFormId = dictionary.MeasurementsMapTypeFormId,
                        Version = 1,
                        IsActive = true,
                        Bindings = rootItems.Select(r => new MeasurementMapDictionaryBinding
                        {
                            MeasurementMapDictionaryId = id,
                            RootItemId = r.Id,
                            RootItemName = r.Izdel,
                            Code_LSF82 = dictionary.Code_LSF82
                        }).ToList()
                    };
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    tran.Rollback();
                    throw new MeasurementMapDictionaryBindingConflictException(
                        "Для одного или нескольких выбранных изделий уже существует " +
                        $"справочник карты измерений на номенклатуру с кодом {dictionary.Code_LSF82}. " +
                        "Выберите другие изделия или другой код номенклатуры.");
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