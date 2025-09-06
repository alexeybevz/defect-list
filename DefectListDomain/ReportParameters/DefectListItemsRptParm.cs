using System;
using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListDomain.ReportParameters
{
    public class DefectListItemsRptParm
    {
        public int BomId { get; set; }
        public string Izdel { get; set; }
        public string IzdelInitial { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberAfterRepair { get; set; }
        public string Contract { get; set; }
        public DateTime? ContractDateOpen { get; set; }
        public IEnumerable<BomItem> BomItems { get; set; }
        public bool IsUseFinalDecision { get; set; }
        public DateTime DateOfPreparation { get; set; }
    }
}