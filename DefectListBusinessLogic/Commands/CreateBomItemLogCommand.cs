using System.Data;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateBomItemLogCommand : ICreateBomItemLogCommand
    {
        public void InsertBomItemLogWithInitValues(int bomItemId, int docId, IDbConnection db,
            IDbTransaction transaction)
        {
            db.Execute(
                "INSERT INTO dbo.BomItemLog (BomItemDocId, BomId, BomItemId, BomItemParentId, Detals, Detal, DetalIma, DetalTyp, DetalUm, QtyMnf, QtyConstr, QtyRestore, QtyReplace, Comment, Defect, Decision, CommentDef, SerialNumber, TechnologicalProcessUsed, FinalDecision, IsRequiredSubmit, IsSubmitted, ResearchAction, ResearchResult, Action, CreateDate, CreatedBy) " +
                "SELECT @BomItemDocId, BomId, Id, ParentId, Detals, Detal, DetalIma, DetalTyp, DetalUm, QtyMnf, QtyConstr, QtyRestore, QtyReplace, Comment, Defect, Decision, CommentDef, SerialNumber, TechnologicalProcessUsed, FinalDecision, ISNULL(IsRequiredSubmit, 0) AS IsRequiredSubmit, ISNULL(IsSubmitted, 0) AS IsSubmitted, ResearchAction, ResearchResult, @Action, CreateDate, CreatedBy " +
                "FROM dbo.BomItem " +
                "WHERE Id = @BomItemId",
                new { BomItemDocId = docId, Action = BomItemLogAction.InsertInit, BomItemId = bomItemId },
                transaction
            );
        }

        public void InsertBomItemLog(IBomItem obj, BomItemLogAction action, string userLogin, int docId, IDbConnection db, IDbTransaction transaction)
        {
            var insertRowLogParameters = new
            {
                BomItemDocId = docId,
                BomId = obj.BomId,
                BomItemId = obj.Id,
                BomItemParentId = obj.ParentId,
                obj.Detals,
                obj.Detal,
                obj.DetalIma,
                obj.DetalTyp,
                obj.DetalUm,
                obj.QtyMnf,
                obj.QtyConstr,
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
                Action = action,
                CreatedBy = userLogin,
            };
            db.Execute(
                "INSERT INTO dbo.BomItemLog (BomItemDocId, BomId, BomItemId, BomItemParentId, Detals, Detal, DetalIma, DetalTyp, DetalUm, QtyMnf, QtyConstr, QtyRestore, QtyReplace, Comment, Defect, Decision, CommentDef, SerialNumber, TechnologicalProcessUsed, FinalDecision, IsRequiredSubmit, IsSubmitted, ResearchAction, ResearchResult, Action, CreatedBy) " +
                "VALUES (@BomItemDocId, @BomId, @BomItemId, @BomItemParentId, @Detals, @Detal, @DetalIma, @DetalTyp, @DetalUm, @QtyMnf, @QtyConstr, @QtyRestore, @QtyReplace, @Comment, @Defect, @Decision, @CommentDef, @SerialNumber, @TechnologicalProcessUsed, @FinalDecision, @IsRequiredSubmit, @IsSubmitted, @ResearchAction, @ResearchResult, @Action, @CreatedBy)",
                insertRowLogParameters,
                transaction
            );
        }
    }
}
