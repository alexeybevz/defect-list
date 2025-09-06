using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateMapBomItemToRouteChartCommand
    {
        Task Execute(MapBomItemToRouteChart mapBomItemToRouteChart);
    }
}