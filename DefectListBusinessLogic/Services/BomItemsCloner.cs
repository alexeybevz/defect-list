using System.Collections.Generic;
using System.Linq;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class BomItemsCloner : IBomItemsCloner
    {
        public void CloneOldBomToNewBom(List<BomItem> initBom, List<BomItem> targetBom)
        {
            if (initBom != null && targetBom != null && initBom.Any() && targetBom.Any())
                Recursive(initBom, targetBom, initBom.First(), targetBom.First(), true);
        }

        private void Recursive(List<BomItem> initBom, List<BomItem> targetBom, BomItem itemFromInitBom, BomItem itemFromTargetBom, bool isRoot = false)
        {
            List<BomItem> initItemBomFirstLevel;
            List<BomItem> targetItemBomFirstLevel;

            if (isRoot)
            {
                initItemBomFirstLevel = initBom.Where(x => x.Id == itemFromInitBom.Id).ToList();
                targetItemBomFirstLevel = targetBom.Where(x => x.Id == itemFromTargetBom.Id).ToList();
            }
            else
            {
                initItemBomFirstLevel = initBom.Where(x => x.ParentId == itemFromInitBom.Id).ToList();
                targetItemBomFirstLevel = targetBom.Where(x => x.ParentId == itemFromTargetBom.Id).ToList();
            }

            foreach (var bi in targetItemBomFirstLevel)
            {
                var obj = initItemBomFirstLevel.FirstOrDefault(x => x.Detal == bi.Detal);
                if (obj == null)
                    continue;

                bi.QtyRestore = obj.QtyRestore;
                bi.QtyReplace = obj.QtyReplace;
                bi.Defect = obj.Defect;
                bi.Decision = obj.Decision;
                bi.FinalDecision = obj.FinalDecision;
                bi.TechnologicalProcessUsed = obj.TechnologicalProcessUsed;
                bi.SerialNumber = obj.SerialNumber;
                bi.CommentDef = obj.CommentDef;
                bi.IsRequiredSubmit = obj.IsRequiredSubmit;
                bi.IsSubmitted = obj.IsSubmitted;
                bi.MapBomItemToRouteCharts = obj.MapBomItemToRouteCharts;

                Recursive(initBom, targetBom, obj, bi);
            }
        }
    }
}