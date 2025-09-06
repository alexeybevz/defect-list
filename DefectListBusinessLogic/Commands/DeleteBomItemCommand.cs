using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteBomItemCommand : DbConnectionPmControlRepositoryBase, IDeleteBomItemCommand
    {
        private readonly ICreateBomItemDocCommand _createBomItemDocCommand;
        private readonly ICreateBomItemLogCommand _createBomItemLogCommand;

        public DeleteBomItemCommand(
            IDbConnectionFactory dbConnectionFactory,
            ICreateBomItemDocCommand createBomItemDocCommand,
            ICreateBomItemLogCommand createBomItemLogCommand) : base(dbConnectionFactory)
        {
            _createBomItemDocCommand = createBomItemDocCommand;
            _createBomItemLogCommand = createBomItemLogCommand;
        }

        public async Task Execute(List<BomItem> bomItems, string userLogin)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                DeleteBomItems(bomItems, userLogin, db);
            }
        }

        public async Task Execute(List<BomItem> bomItems, string userLogin, IDbConnection db, IDbTransaction transaction)
        {
            DeleteBomItems(bomItems, userLogin, db, transaction);
        }

        private void DeleteBomItems(List<BomItem> bomItems, string userLogin, IDbConnection db, IDbTransaction transaction = null)
        {
            if (!bomItems.Any())
                return;

            string tempTableName = "##DltBomItem_" + Guid.NewGuid().ToString().Replace("-", "_");

            db.Execute($"CREATE TABLE {tempTableName} (Id int PRIMARY KEY)", transaction: transaction);

            var bomItemsDataTable = bomItems.Select(x => new { x.Id }).ToList().ToDataTable();

            using (var loader = new SqlBulkCopy((SqlConnection)db, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
            {
                loader.BatchSize = bomItems.Count;
                loader.DestinationTableName = tempTableName;
                loader.WriteToServer(bomItemsDataTable);
            }

            db.Execute($"DELETE FROM BomItem WHERE Id IN (SELECT Id FROM {tempTableName})", transaction: transaction, commandTimeout: 150);

            foreach (var bomItem in bomItems)
            {
                var docId = _createBomItemDocCommand.Execute(bomItem.Id, userLogin, db, transaction);
                _createBomItemLogCommand.InsertBomItemLog(bomItem, BomItemLogAction.Delete, userLogin, docId, db, transaction);
            }

            db.Execute($"DROP TABLE {tempTableName}", transaction: transaction);
        }
    }
}