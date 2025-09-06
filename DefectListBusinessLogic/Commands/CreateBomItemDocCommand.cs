using System.Data;
using Dapper;
using DefectListDomain.Commands;

namespace DefectListBusinessLogic.Commands
{
    public class CreateBomItemDocCommand : ICreateBomItemDocCommand
    {
        private readonly ICreateBomItemLogCommand _createBomItemLogCommand;

        public CreateBomItemDocCommand(ICreateBomItemLogCommand createBomItemLogCommand)
        {
            _createBomItemLogCommand = createBomItemLogCommand;
        }

        public int Execute(int bomItemId, string userLogin, IDbConnection db, IDbTransaction transaction)
        {
            int docId = 0;

            var result = db.ExecuteScalar("SELECT DocId FROM dbo.BomItemDoc WHERE BomItemId = @BomItemId AND Status = 0",
                new { BomItemId = bomItemId },
                transaction);
            if (result != null)
                docId = (int)result;
            else
            {
                var createDocParameters = new
                {
                    BomItemId = bomItemId,
                    Status = 0,
                    CreatedBy = userLogin,
                    UpdatedBy = userLogin,
                };
                docId = db.QuerySingle<int>("INSERT INTO dbo.BomItemDoc (BomItemId, Status, CreatedBy, UpdatedBy) " +
                                            "VALUES (@BomItemId, @Status, @CreatedBy, @UpdatedBy) " +
                                            "SELECT CAST(SCOPE_IDENTITY() AS int)", createDocParameters, transaction);

                _createBomItemLogCommand.InsertBomItemLogWithInitValues(bomItemId, docId, db, transaction);
            }

            return docId;
        }
    }
}