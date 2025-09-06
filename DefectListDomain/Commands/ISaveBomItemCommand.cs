using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ISaveBomItemCommand
    {
        void Execute(IBomItem selectedDefectListItem, string userLogin);
    }
}