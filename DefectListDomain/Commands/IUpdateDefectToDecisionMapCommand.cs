using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUpdateDefectToDecisionMapCommand
    {
        Task Execute(MapDefectToDecision mapDefectToDecision);
    }
}