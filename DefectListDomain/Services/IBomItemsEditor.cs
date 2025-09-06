using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.EventArgs;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IBomItemsEditor
    {
        Task Add(BomHeader bomHeader, BomItem parentBomItem, BomItem addBomItem, string login);
        Task Replace(BomHeader bomHeader, BomItem oldBomItem, BomItem newBomItem, List<BomItem> bom, string login);
        Task ReplaceName(BomHeader bomHeader, BomItem oldBomItem, string newDetal, string login);
        Task Delete(BomHeader bomHeader, BomItem deletedBomItem, List<BomItem> deletedBomItems, string login);
        Task Expand(int assemblyBomItemId, string assemblyBomItemStructureNumber, List<BomItem> assemblyBom, bool isExpandAll = false);
        Task Collapse(int assemblyBomItemId, List<BomItem> assemblyBom);

        event EventHandler<ErrorEventArgs> OnError;
    }
}