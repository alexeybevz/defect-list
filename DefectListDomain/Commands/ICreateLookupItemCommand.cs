using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateLookupItemCommand
    {
        Task<int> ExecuteAsync(LookupItemKind kind, string name, string createdBy);
    }
}