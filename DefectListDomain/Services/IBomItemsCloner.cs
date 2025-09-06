using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IBomItemsCloner
    {
        void CloneOldBomToNewBom(List<BomItem> initBom, List<BomItem> targetBom);
    }
}