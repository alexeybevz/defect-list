using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllStateDetalsQuery
    {
        Task<IEnumerable<StateDetals>> Execute();
    }
}