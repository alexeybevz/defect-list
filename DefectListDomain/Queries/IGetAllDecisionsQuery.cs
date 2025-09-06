using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefectListDomain.Queries
{
    public interface IGetAllDecisionsQuery
    {
        Task<IEnumerable<string>> Execute();
    }
}