using System.Data;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateBomItemNameCommand : DbConnectionPmControlRepositoryBase, IUpdateBomItemNameCommand
    {
        private readonly ICreateBomItemDocCommand _createBomItemDocCommand;
        private readonly ICreateBomItemLogCommand _createBomItemLogCommand;

        public UpdateBomItemNameCommand(
            IDbConnectionFactory dbConnectionFactory,
            ICreateBomItemDocCommand createBomItemDocCommand,
            ICreateBomItemLogCommand createBomItemLogCommand) : base(dbConnectionFactory)
        {
            _createBomItemDocCommand = createBomItemDocCommand;
            _createBomItemLogCommand = createBomItemLogCommand;
        }

        public void Execute(int bomItemId, IBomItem newBomItem, string userLogin)
        {
            var updateBomItemParameters = new
            {
                newBomItem.Detals,
                newBomItem.Detal,
                newBomItem.DetalIma,
                newBomItem.DetalTyp,
                newBomItem.DetalUm,
                newBomItem.ClassifierID,
                newBomItem.ProductID,
                newBomItem.Code_LSF82,
                UpdatedBy = userLogin,
                Id = bomItemId,
            };

            using (var db = CreateOpenConnection())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var docId = _createBomItemDocCommand.Execute(bomItemId, userLogin, db, transaction);

                    db.Execute(
                        "UPDATE dbo.BomItem SET Detals = @Detals, Detal = @Detal, DetalIma = @DetalIma, " +
                        "DetalTyp = @DetalTyp, DetalUm = @DetalUm, ClassifierID = @ClassifierID, ProductID = @ProductID, Code_LSF82 = @Code_LSF82, " +
                        "RecordDate = GETDATE(), UpdatedBy = @UpdatedBy " +
                        "WHERE Id = @Id",
                        updateBomItemParameters,
                        transaction);

                    _createBomItemLogCommand.InsertBomItemLog(newBomItem, BomItemLogAction.Update, userLogin, docId, db, transaction);

                    transaction.Commit();
                }
            }
        }
    }
}