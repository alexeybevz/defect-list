using System.Collections.Generic;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IRouteMapFactory
    {
        void AddTasksOnCreateRouteMaps(List<RouteMapIntegrationTaskDto> tasks);
        List<RouteMapIntegrationTaskDto> AskHandledTasks(string sessionGuid);
        void RunMarKartaExeToHandleTasksOnCreate(string sessionGuid);
        void RunMarKartaExeToHandleTasksOnDelete(string sessionGuid);
        void RunMarKartaExeToHandleTasksOnReturnKolotryvParentChild(string sessionGuid);
        void RunMarKartaExeToHandleTasksOnWriteOffKolotryv(string sessionGuid);
    }
}