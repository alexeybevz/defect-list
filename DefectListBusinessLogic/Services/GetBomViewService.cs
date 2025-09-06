using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class GetBomViewService : IGetBomViewService
    {
        public async Task<IEnumerable<BomItem>> Execute(IEnumerable<BomItem> bomItems)
        {
            return await Task.Run(() =>
            {
                bomItems = bomItems?.ToList() ?? new List<BomItem>();

                if (!bomItems.Any())
                    return bomItems;

                var root = bomItems.First(x => x.ParentId == null);
                root.StructureNumber = "1";

                RecursiveStructure(bomItems, root.Id, root.QtyMnf, root.StructureNumber);
                return bomItems.OrderBy(x => x.StructureNumber).ToList();
            });
        }

        private void RecursiveStructure(IEnumerable<BomItem> bomItems, int id, float qtyMnf, string structureNumber)
        {
            var childs = bomItems.Where(x => x.ParentId == id).OrderBy(x => x.OrderByPriority).ThenBy(x => x.Detal).ToList();
            for (var i = 0; i < childs.Count; i++)
            {
                var obj = childs[i];
                obj.QtyMnf *= qtyMnf;
                obj.StructureNumber = CreateStructureNumber(structureNumber, i + 1);
                RecursiveStructure(bomItems, obj.Id, obj.QtyMnf, obj.StructureNumber);
            }
        }

        private string CreateStructureNumber(string structure, int i)
        {
            string str = i.ToString();
            return structure + "." + new string(' ', 4 - str.Length) + str;
        }
    }
}