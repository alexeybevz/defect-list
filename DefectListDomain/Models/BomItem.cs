using System;
using System.Collections.Generic;

namespace DefectListDomain.Models
{
    public class BomItem : IBomItem, ICloneable
    {
        public int BomId { get; set; }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string ParentDetal { get; set; }
        public string ParentDetalIma { get; set; }
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
        public List<MapBomItemToRouteChart> MapBomItemToRouteCharts { get; set; }
        public bool? IsExpanded { get; set; }
        public bool IsShowItem { get; set; }
        public string ClassifierID { get; set; }
        public int? ProductID { get; set; }
        public int? Code_LSF82 { get; set; }
        public bool IsExpensive { get; set; }
        public string ResearchAction { get; set; }
        public string ResearchResult { get; set; }

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
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public string RepairMethodName { get; set; }
        public string StructureNumber { get; set; }
        public int OrderByPriority
        {
            get
            {
                switch (DetalTyp)
                {
                    case "издел": return 0;
                    case "матер": return 2;
                    case "сб.ед": return 3;
                    default: return 1;
                }
            }
        }

        public byte UzelFlag { get; set; }
        public bool IsPki => DetalTyp == "покуп";
        public bool IsMaterial => DetalTyp == "матер";
        public bool IsOtlivka => DetalTyp == "литье";
        public bool IsScrap => Decision?.ToLower() == "заменить";
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public interface IBomItem
    {
        int BomId { get; set; }
        int Id { get; set; }
        int? ParentId { get; set; }
        string ParentDetal { get; set; }
        string ParentDetalIma { get; set; }
        string Detals { get; set; }
        string Detal { get; set; }
        string DetalIma { get; set; }
        string DetalTyp { get; set; }
        string DetalUm { get; set; }
        float QtyMnf { get; set; }
        float QtyConstr { get; set; }
        float QtyRestore { get; set; }
        float QtyReplace { get; set; }
        string Comment { get; set; }
        string Defect { get; set; }
        string Decision { get; set; }
        string CommentDef { get; set; }
        string SerialNumber { get; set; }
        string TechnologicalProcessUsed { get; set; }
        string FinalDecision { get; set; }
        IsBomItemRequiredSubmit IsRequiredSubmit { get; set; }
        string IsRequiredSubmitText { get; }
        IsBomItemSubmitted IsSubmitted { get; set; }
        string IsSubmittedText { get; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        string CreatedByName { get; set; }
        DateTime RecordDate { get; set; }
        string UpdatedBy { get; set; }
        string UpdatedByName { get; set; }
        string RepairMethodName { get; set; }
        string StructureNumber { get; set; }
        int OrderByPriority { get; }
        byte UzelFlag { get; set; }
        bool IsPki { get; }
        bool IsMaterial { get; }
        bool IsOtlivka { get; }
        bool IsScrap { get; }
        List<MapBomItemToRouteChart> MapBomItemToRouteCharts { get; set; }
        bool? IsExpanded { get; set; }
        bool IsShowItem { get; set; }
        string ClassifierID { get; set; }
        int? ProductID { get; set; }
        int? Code_LSF82 { get; set; }
        bool IsExpensive { get; set; }
        string ResearchAction { get; set; }
        string ResearchResult { get; set; }
    }

    public interface IBomItemModel : IBomItem
    {
        bool IsFilled { get; }
        bool IsOnlyRestore { get; }
        bool IsOnlyReplace { get; }
        bool IsRestoreAndReplace { get; }
        bool IsAcceptive { get; }
        bool IsExistsRepairTechnologicalProcess { get; }
        bool IsExistsReplaceTechnologicalProcess { get; }
        bool IsSelected { get; set; }
    }

    public enum IsBomItemRequiredSubmit
    {
        Yes = 1,
        No = 0
    }

    public enum IsBomItemSubmitted
    {
        Yes = 1,
        No = 0
    }
}