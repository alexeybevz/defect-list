using System.Collections.Generic;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllPlanOperationDtoQuery : IGetAllPlanOperationDtoQuery
    {
        public List<PlanOperationDto> AskPlanOperations(string detals)
        {
            return new List<PlanOperationDto>();
        }

        public bool IsTehnologicheskayaSborka(string detals)
        {
            return false;
        }
    }
}