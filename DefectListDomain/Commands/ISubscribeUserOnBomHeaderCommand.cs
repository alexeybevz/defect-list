using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ISubscribeUserOnBomHeaderCommand
    {
        Task Execute(BomHeaderSubscriber bomHeaderSubscriber);
    }
}