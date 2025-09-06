using System;

namespace DefectListDomain.Models
{
    public class BomItemLog
    {
        public int Id { get; set; }
        public int BomItemDocId { get; set; }
        public int BomItemId { get; set; }
        public int BomItemParentId { get; set; }
        public string Detals { get; set; }
        public string Detal { get; set; }
        public string DetalIma { get; set; }
        public string DetalTyp { get; set; }
        public string DetalUm { get; set; }
        public float QtyMnf { get; set; }
        public float QtyConstr { get; set; }
        public float QtyRestore { get; set; }
        public float QtyReplace { get; set; }
        public string Comment { get; set; }
        public string Defect { get; set; }
        public string Decision { get; set; }
        public string CommentDef { get; set; }
        public string SerialNumber { get; set; }
        public string TechnologicalProcessUsed { get; set; }
        public string FinalDecision { get; set; }
        public IsBomItemRequiredSubmit IsRequiredSubmit { get; set; }
        public string IsRequiredSubmitText
        {
            get
            {
                switch (IsRequiredSubmit)
                {
                    case IsBomItemRequiredSubmit.Yes: return "Требуется предъявить ВП";
                    case IsBomItemRequiredSubmit.No: return "Не требует предъявления ВП";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public IsBomItemSubmitted IsSubmitted { get; set; }
        public string IsSubmittedText
        {
            get
            {
                switch (IsSubmitted)
                {
                    case IsBomItemSubmitted.Yes: return "Предъявлено ВП";
                    case IsBomItemSubmitted.No: return "Не предъявлено ВП";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public string ResearchAction { get; set; }
        public string ResearchResult { get; set; }
        public byte Action { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}