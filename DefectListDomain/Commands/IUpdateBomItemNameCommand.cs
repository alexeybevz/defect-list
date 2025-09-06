using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUpdateBomItemNameCommand
    {
        void Execute(int bomItemId, IBomItem newObj, string userLogin);
    }
}