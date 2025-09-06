using System.Collections.Generic;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllAuxiliaryMaterialDtoQuery : IGetAllAuxiliaryMaterialDtoQuery
    {
        public IEnumerable<AuxiliaryMaterialDto> Execute()
        {
            return new List<AuxiliaryMaterialDto>();
        }
    }
}