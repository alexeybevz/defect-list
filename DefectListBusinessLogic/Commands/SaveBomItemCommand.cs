using System.Data;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class SaveBomItemCommand : DbConnectionPmControlRepositoryBase, ISaveBomItemCommand
    {
        private readonly ICreateBomItemDocCommand _createBomItemDocCommand;
        private readonly ICreateBomItemLogCommand _createBomItemLogCommand;

        public SaveBomItemCommand(
            IDbConnectionFactory dbConnectionFactory,
            ICreateBomItemDocCommand createBomItemDocCommand,
            ICreateBomItemLogCommand createBomItemLogCommand) : base(dbConnectionFactory)
        {
            _createBomItemDocCommand = createBomItemDocCommand;
            _createBomItemLogCommand = createBomItemLogCommand;
        }

        public void Execute(IBomItem selectedDefectListItem, string userLogin)
        {
            using (var db = CreateOpenConnection())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var docId = _createBomItemDocCommand.Execute(selectedDefectListItem.Id, userLogin, db, transaction);

                    UpdateBomItem(selectedDefectListItem, userLogin, db, transaction);
                    _createBomItemLogCommand.InsertBomItemLog(selectedDefectListItem, BomItemLogAction.Update, userLogin, docId, db, transaction);

                    transaction.Commit();
                }
            }
        }

        private void UpdateBomItem(IBomItem obj, string userLogin, IDbConnection db,
            IDbTransaction transaction)
        {
            var updateBomItemParameters = new
            {
                obj.QtyRestore,
                obj.QtyReplace,
                obj.Comment,
                obj.Defect,
                obj.Decision,
                obj.CommentDef,
                obj.SerialNumber,
                obj.TechnologicalProcessUsed,
                obj.FinalDecision,
                obj.IsRequiredSubmit,
                obj.IsSubmitted,
                obj.ResearchAction,
                obj.ResearchResult,
                UpdatedBy = userLogin,
                obj.Id,
            };

            db.Execute(
                "UPDATE dbo.BomItem SET QtyRestore = @QtyRestore, QtyReplace = @QtyReplace, Comment = @Comment, " +
                "Defect = @Defect, Decision = @Decision, CommentDef = @CommentDef, SerialNumber = @SerialNumber, TechnologicalProcessUsed = @TechnologicalProcessUsed, FinalDecision = @FinalDecision, IsRequiredSubmit = @IsRequiredSubmit, IsSubmitted = @IsSubmitted, " +
                "ResearchAction = @ResearchAction, ResearchResult = @ResearchResult, " +
                "RecordDate = GETDATE(), UpdatedBy = @UpdatedBy " +
                "WHERE Id = @Id",
                updateBomItemParameters,
                transaction);
        }
    }
}