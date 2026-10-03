using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListWpfControl.DefectList.Stores
{
    public class ProductsStore
    {
        private readonly IGetAllProductDtoQuery _getAllProductDtoQuery;
        private readonly IGetAllDistinctShopEntriesQuery _getAllDistinctShopEntriesQuery;

        public ProductsStore(
            IGetAllProductDtoQuery getAllProductDtoQuery,
            IGetAllDistinctShopEntriesQuery getAllDistinctShopEntriesQuery)
        {
            _getAllProductDtoQuery = getAllProductDtoQuery;
            _getAllDistinctShopEntriesQuery = getAllDistinctShopEntriesQuery;
        }

        public async Task<IEnumerable<ProductDto>> GetAllDesignSpecifications()
        {
            return await _getAllProductDtoQuery.ExecuteDesignSpecification();
        }

        public async Task<ProductDto> GetProductByDetals(string detals)
        {
            return await _getAllProductDtoQuery.ExecuteByDetals(detals);
        }

        public async Task<Dictionary<int, string>> GetAllDistinctShopEntries()
        {
            return await _getAllDistinctShopEntriesQuery.Execute();
        }
    }
}