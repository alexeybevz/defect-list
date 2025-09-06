using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IBomItemsValidator
    {
        string Execute(List<BomItem> bomItems, IBomItem bomItem);
    }
}