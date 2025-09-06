using System;
using System.Collections.Generic;
using DefectListDomain.Dtos;

namespace DefectListDomain.ExternalData
{
    public interface IAsupBomContextFactory
    {
        List<AsupBomComponentDto> AskBase(string rootProductName, decimal rootProductQty, DateTime? specifOnDate = null);
    }
}