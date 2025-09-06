using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IBomItemsLoader
    {
        Task<int> Execute(
            List<BomItem> deleteBomItems,
            List<AsupBomComponentDto> insertNewBomItems,
            int bomId,
            int? parentId,
            float parentQty,
            float selfQty,
            string userLogin);
    }
}