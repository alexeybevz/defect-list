using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IDeleteMapBomItemToRouteChartCommand
    {
        Task Execute(string routeChartNumber);
    }
}