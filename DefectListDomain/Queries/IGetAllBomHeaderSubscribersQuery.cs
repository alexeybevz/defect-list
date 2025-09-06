using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomHeaderSubscribersQuery
    {
        Task<IEnumerable<BomHeaderSubscriber>> Execute();
    }
}