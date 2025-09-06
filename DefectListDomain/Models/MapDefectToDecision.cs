namespace DefectListDomain.Models
{
    public class MapDefectToDecision
    {
        public int Id { get; set; }
        public string Defect { get; set; }
        public string Decision { get; set; }
        public bool IsAllowCombine { get; set; }
        public StateDetals StateDetals { get; set; }
        public GroupDefect GroupDefect { get; set; }
    }
}