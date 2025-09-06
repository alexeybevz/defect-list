using System.Collections.Generic;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllAuxiliaryMaterialDtoQuery
    {
        IEnumerable<AuxiliaryMaterialDto> Execute();
    }
}