using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IGetBomViewService
    {
        Task<IEnumerable<BomItem>> Execute(IEnumerable<BomItem> bomItems);
    }
}