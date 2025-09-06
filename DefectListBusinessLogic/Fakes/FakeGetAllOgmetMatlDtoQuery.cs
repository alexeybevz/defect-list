using System.Collections.Generic;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllOgmetMatlDtoQuery : IGetAllOgmetMatlDtoQuery
    {
        public List<OgmetMatlDto> AskMatls()
        {
            return new List<OgmetMatlDto>();
        }
    }
}