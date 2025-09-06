using System.Collections.Generic;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllOgmetMatlDtoQuery
    {
        List<OgmetMatlDto> AskMatls();
    }
}