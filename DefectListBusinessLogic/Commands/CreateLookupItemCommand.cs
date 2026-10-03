using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Exceptions;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class CreateLookupItemCommand : DbConnectionPmControlRepositoryBase, ICreateLookupItemCommand
    {
        public CreateLookupItemCommand(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory) { }

        public async Task<int> ExecuteAsync(LookupItemKind kind, string name, string createdBy)
        {
            try
            {
                var tableName = GetTableName(kind);
                using (var db = await CreateOpenConnectionAsync())
                {
                    return await db.ExecuteScalarAsync<int>(
                        $"INSERT INTO {tableName} (Name, IsActive) VALUES (@Name, 1); SELECT CAST(SCOPE_IDENTITY() AS int);",
                        new { Name = name });
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new UniqueConstraintViolationException("Объект с таким наименованием уже существует.");
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