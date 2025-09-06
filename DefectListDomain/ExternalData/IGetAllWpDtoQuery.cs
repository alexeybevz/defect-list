using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllWpDtoQuery
    {
        Task<IEnumerable<WpDto>> ExecuteAsync();
    }
}