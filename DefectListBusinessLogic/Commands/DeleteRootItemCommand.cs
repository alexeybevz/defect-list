using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteRootItemCommand : DbConnectionPmControlRepositoryBase, IDeleteRootItemCommand
    {
        public DeleteRootItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int id)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"DELETE FROM RootItem WHERE Id = @Id";

                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        await db.ExecuteAsync(query, new { Id = id }, transaction);
                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 547)
                            throw new Exception(
                                "Удаление ремонтного изделия запрещено, т.к. оно используется в дефектовочных ведомостях.");
                        throw;
                    }
                }
            }
        }
    }
}