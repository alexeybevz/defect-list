using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IDeleteBomItemCommand
    {
        Task Execute(List<BomItem> bomItems, string userLogin);
        Task Execute(List<BomItem> bomItems, string userLogin, IDbConnection db, IDbTransaction transaction);
    }
}