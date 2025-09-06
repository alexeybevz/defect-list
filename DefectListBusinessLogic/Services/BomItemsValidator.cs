using System.Collections.Generic;
using DefectListDomain.Models;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class BomItemsValidator : IBomItemsValidator
    {
        public string Execute(List<BomItem> bomItems, IBomItem bomItem)
        {
            if (bomItem.QtyReplace > bomItem.QtyMnf || bomItem.QtyRestore > bomItem.QtyMnf)
                return
                    "ДСЕ не может ремонтироваться или менятся в большем количестве, чем указано в плановом количестве";

            return null;
        }
    }
}
