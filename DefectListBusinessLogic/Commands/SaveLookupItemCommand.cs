using System;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class SaveLookupItemCommand : DbConnectionPmControlRepositoryBase, ISaveLookupItemCommand
    {
        public SaveLookupItemCommand(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory) { }

        public async Task<int> ExecuteAsync(LookupItemKind kind, int id, string newName, bool isUsed, string updatedBy)
        {
            var tableName = GetTableName(kind);

            using (var db = await CreateOpenConnectionAsync())
            {
                if (!isUsed)
                {
                    // Запись не используется — просто переименовываем
                    await db.ExecuteAsync(
                        $"UPDATE {tableName} SET Name = @Name WHERE Id = @Id",
                        new { Name = newName, Id = id });

                    return id;
                }
                else
                {
                    // Запись используется — деактивируем старую, создаём новую
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            await db.ExecuteAsync(
                                $"UPDATE {tableName} SET IsActive = 0 WHERE Id = @Id",
                                new { Id = id },
                                tran);

                            var newId = await db.ExecuteScalarAsync<int>(
                                $"INSERT INTO {tableName} (Name, IsActive) VALUES (@Name, 1); SELECT CAST(SCOPE_IDENTITY() AS int);",
                                new { Name = newName },
                                tran);

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
    }
}