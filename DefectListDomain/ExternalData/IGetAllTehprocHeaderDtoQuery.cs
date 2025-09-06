using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllTehprocHeaderDtoQuery
    {
        Task<IEnumerable<TehprocHeaderDto>> Execute();
        Task<TehprocHeaderDto> ExecuteByDetals(string detals);
    }
}