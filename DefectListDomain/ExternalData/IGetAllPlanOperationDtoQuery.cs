using System.Collections.Generic;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllPlanOperationDtoQuery
    {
        List<PlanOperationDto> AskPlanOperations(string detals);
        bool IsTehnologicheskayaSborka(string detals);
    }
}