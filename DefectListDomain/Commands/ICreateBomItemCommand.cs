using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DefectListDomain.Dtos;

namespace DefectListDomain.Commands
{
    public interface ICreateBomItemCommand
    {
        Task<int> Execute(
            int bomId,
            ICollection<AsupBomComponentDto> bomToLoad,
            int? parentItemId,
            float parentQty,
            float selfQty,
            string userLogin);

        Task<int> Execute(
            int bomId,
            ICollection<AsupBomComponentDto> bomToLoad,
            int? parentItemId,
            float parentQty,
            float selfQty,
            string userLogin,
            IDbConnection db,
            IDbTransaction transaction);
    }
}