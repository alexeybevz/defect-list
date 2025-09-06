using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IDeleteDefectToDecisionMapCommand
    {
        Task Execute(int id);
    }
}