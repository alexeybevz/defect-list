using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllProductDtoQuery
    {
        Task<IEnumerable<ProductDto>> Execute();
        Task<IEnumerable<ProductDto>> ExecuteDesignSpecification();
        Task<ProductDto> ExecuteByProductId(int productId);
        Task<ProductDto> ExecuteByCodeLsf82(int codeLsf82);
        Task<ProductDto> ExecuteByDetals(string detals);
    }
}