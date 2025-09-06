using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomHeadersQuery
    {
        Task<IEnumerable<BomHeader>> Execute();
        Task<BomHeader> Execute(int id);
    }
}