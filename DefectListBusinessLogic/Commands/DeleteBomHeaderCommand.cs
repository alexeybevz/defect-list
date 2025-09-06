using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteBomHeaderCommand : DbConnectionPmControlRepositoryBase, IDeleteBomHeaderCommand
    {
        public DeleteBomHeaderCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int id)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                try
                {
                    var query = @"DELETE FROM dbo.BomHeader WHERE BomId = @BomId";

                    using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        await db.ExecuteAsync(query, new { BomId = id }, transaction);
                        transaction.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Errors.Count > 0)
                    {
                        switch (ex.Errors[0].Number)
                        {
                            case 547:
                                throw new InvalidOperationException("Нельзя удалить дефектовочную ведомость.\nПричина: дефектовочная ведомость содержит состав.");
                        }
                    }
                }
            }
        }
    }
}