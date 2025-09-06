using System;

namespace DefectListDomain.Models
{
    public class FinalDecisionChanging
    {
        public int BomId { get; set; }
        public string Orders { get; set; }
        public string Izdel { get; set; }
        public string Detal { get; set; }
        public string DetalIma { get; set; }
        public string DetalTyp { get; set; }
        public decimal QtyMnf { get; set; }
        public string DetalUm { get; set; }
        public string FinalDecision { get; set; }
        public string NextFinalDecision { get; set; }
        public string Nomgodurs { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedByName { get; set; }
    }
}