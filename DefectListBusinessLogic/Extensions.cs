using System.Collections.Generic;
using System.Linq;
using DefectListDomain.Models;

namespace DefectListBusinessLogic
{
    public static class Extensions
    {
        public static IQueryable<BomItem> IsShowed(this IEnumerable<BomItem> bomItems)
        {
            return bomItems.Where(x => x.IsShowItem).AsQueryable();
        }
    }
}