using System.Data;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateBomItemLogCommand
    {
        void InsertBomItemLogWithInitValues(int bomItemId, int docId, IDbConnection db, IDbTransaction transaction);

        void InsertBomItemLog(IBomItem obj, BomItemLogAction action, string userLogin, int docId, IDbConnection db, IDbTransaction transaction);
    }
}