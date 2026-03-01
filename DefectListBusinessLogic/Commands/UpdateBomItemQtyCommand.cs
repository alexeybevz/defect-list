using System.Data;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateBomItemQtyCommand : DbConnectionPmControlRepositoryBase, IUpdateBomItemQtyCommand
    {
        private readonly ICreateBomItemDocCommand _createBomItemDocCommand;
        private readonly ICreateBomItemLogCommand _createBomItemLogCommand;

        public UpdateBomItemQtyCommand(
            IDbConnectionFactory dbConnectionFactory,
            ICreateBomItemDocCommand createBomItemDocCommand,
            ICreateBomItemLogCommand createBomItemLogCommand) : base(dbConnectionFactory)
        {
            _createBomItemDocCommand = createBomItemDocCommand;
            _createBomItemLogCommand = createBomItemLogCommand;
        }

        public async Task Execute(int bomItemId, BomItem bomItem, string userLogin)
        {
            using (var db = CreateOpenConnection())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var docId = _createBomItemDocCommand.Execute(bomItemId, userLogin, db, transaction);

                    var updateBomItemParameters = new
                    {
                        QtyMnf = bomItem.QtyMnf,
                        UpdatedBy = userLogin,
                        Id = bomItemId,
                    };

                    db.Execute(
                        "UPDATE dbo.BomItem SET QtyMnf = @QtyMnf, " +
                        "RecordDate = GETDATE(), UpdatedBy = @UpdatedBy " +
                        "WHERE Id = @Id",
                        updateBomItemParameters,
                        transaction);

                    _createBomItemLogCommand.InsertBomItemLog(bomItem, BomItemLogAction.Update, userLogin, docId, db, transaction);

                    transaction.Commit();
                }
            }
        }
    }
}