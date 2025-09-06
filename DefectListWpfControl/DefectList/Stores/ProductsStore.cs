using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListWpfControl.DefectList.Stores
{
    public class ProductsStore
    {
        private readonly IGetAllProductDtoQuery _getAllProductDtoQuery;

        public ProductsStore(IGetAllProductDtoQuery getAllProductDtoQuery)
        {
            _getAllProductDtoQuery = getAllProductDtoQuery;
        }

        public async Task<IEnumerable<ProductDto>> GetAllDesignSpecifications()
        {
            return await _getAllProductDtoQuery.ExecuteDesignSpecification();
        }

        public async Task<ProductDto> GetProductByDetals(string detals)
        {
            return await _getAllProductDtoQuery.ExecuteByDetals(detals);
        }
    }
}