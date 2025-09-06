using System.Data;

namespace DefectListDomain.Commands
{
    public interface ICreateBomItemDocCommand
    {
        int Execute(int bomItemId, string userLogin, IDbConnection db, IDbTransaction transaction);
    }
}