using System;

namespace DefectListDomain.Models
{
    public class LogRecord
    {
        public string Orders { get; set; }
        public string Detal { get; set; }
        public string DetalIma { get; set; }
        public string Defect { get; set; }
        public string Decision { get; set; }
        public string FinalDecision { get; set; }
        public string SerialNumber { get; set; }
        public string DefectNew { get; set; }
        public string DecisionNew { get; set; }
        public string FinalDecisionNew { get; set; }
        public string SerialNumberNew { get; set; }
        public string ActionText { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
    }
}