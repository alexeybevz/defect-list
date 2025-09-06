using System.Collections.Generic;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeRouteMapFactory : IRouteMapFactory
    {
        public void AddTasksOnCreateRouteMaps(List<RouteMapIntegrationTaskDto> tasks)
        {
        }

        public List<RouteMapIntegrationTaskDto> AskHandledTasks(string sessionGuid)
        {
            return new List<RouteMapIntegrationTaskDto>();
        }

        public void RunMarKartaExeToHandleTasksOnCreate(string sessionGuid)
        {
        }

        public void RunMarKartaExeToHandleTasksOnDelete(string sessionGuid)
        {
        }

        public void RunMarKartaExeToHandleTasksOnReturnKolotryvParentChild(string sessionGuid)
        {
        }

        public void RunMarKartaExeToHandleTasksOnWriteOffKolotryv(string sessionGuid)
        {
        }
    }
}