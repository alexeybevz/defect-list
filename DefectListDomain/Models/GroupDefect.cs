namespace DefectListDomain.Models
{
    public class GroupDefect
    {
        public int Id { get; set; }
        public string GroupDefectName { get; set; }

        public override string ToString()
        {
            return GroupDefectName;
        }
    }
}