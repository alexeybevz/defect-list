using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUnSubscribeUserOnBomHeaderCommand
    {
        Task Execute(BomHeaderSubscriber bomHeaderSubscriber);
    }
}