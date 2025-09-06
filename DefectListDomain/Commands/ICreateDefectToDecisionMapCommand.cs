using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateDefectToDecisionMapCommand
    {
        Task Execute(MapDefectToDecision mapDefectToDecision);
    }
}