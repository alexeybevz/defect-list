using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomItemsByBomHeaderQuery
    {
        Task<IEnumerable<BomItem>> Execute(int bomId);
        Task<IEnumerable<BomItem>> Execute(int bomId, IDbConnection db, IDbTransaction transaction);
        Task<BomItem> ExecuteById(int id);
        Task<BomItem> ExecuteById(int id, IDbConnection db, IDbTransaction transaction);
        Task<IEnumerable<BomItem>> ExecuteByProductId(int productId);
    }
}