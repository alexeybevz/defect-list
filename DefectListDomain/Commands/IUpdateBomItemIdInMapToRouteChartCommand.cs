using System.Data;
using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IUpdateBomItemIdInMapToRouteChartCommand
    {
        Task Execute(int oldBomItemId, int newBomItemId);

        void Execute(int oldBomItemId, int newBomItemId,
            IDbConnection db, IDbTransaction transaction);
    }
}