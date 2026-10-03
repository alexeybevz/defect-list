using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    // ─────────────────────────────────────────────────────────────────
    // Архивирование справочника (IsActive = 0)
    // ─────────────────────────────────────────────────────────────────
    public class ArchiveMeasurementMapDictionaryCommand : DbConnectionPmControlRepositoryBase, IArchiveMeasurementMapDictionaryCommand
    {
        private readonly IGetMeasurementMapDictionaryByIdQuery _getMeasurementMapDictionaryByIdQuery;

        public ArchiveMeasurementMapDictionaryCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetMeasurementMapDictionaryByIdQuery getMeasurementMapDictionaryByIdQuery) : base(dbConnectionFactory)
        {
            _getMeasurementMapDictionaryByIdQuery = getMeasurementMapDictionaryByIdQuery;
        }

        public async Task ExecuteAsync(int dictionaryId, string updatedBy)
        {
            const string updateSql = @"
                UPDATE MeasurementMapDictionary
                SET IsActive = 0, RecordDate = GETDATE(), UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            const string logSql = @"
                INSERT INTO MeasurementMapDictionaryLog
                    (MeasurementMapDictionaryId, Action, Name, Code_LSF82,
                     MeasurementsMapTypeFormId, Version, IsActive, Comment, BoundRootItems, CreateDate, CreatedBy)
                VALUES
                    (@Id, 2, @Name, @Code_LSF82, @FormId, @Version, 0, @Comment, @BoundRootItems, GETDATE(), @UpdatedBy)";

            const string deleteBindingsSql = @"DELETE FROM MeasurementMapDictionaryBinding WHERE MeasurementMapDictionaryId = @MeasurementMapDictionaryId";

            var snap = await _getMeasurementMapDictionaryByIdQuery.ExecuteAsync(dictionaryId);

            using (var db = await CreateOpenConnectionAsync())
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    await db.ExecuteAsync(updateSql, new { Id = dictionaryId, UpdatedBy = updatedBy }, tran);

                    await db.ExecuteAsync(logSql, new
                    {
                        Id = dictionaryId,
                        Name = snap.Name,
                        Code_LSF82 = snap.Code_LSF82,
                        FormId = snap.MeasurementsMapTypeFormId,
                        Version = snap.Version,
                        Comment = snap.Comment,
                        BoundRootItems = snap.BoundRootItemNames,
                        UpdatedBy = updatedBy
                    }, tran);

                    await db.ExecuteAsync(deleteBindingsSql, new {MeasurementMapDictionaryId = dictionaryId}, tran);
                    
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