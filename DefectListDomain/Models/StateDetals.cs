namespace DefectListDomain.Models
{
    public class StateDetals
    {
        public int Id { get; set; }
        public string StateDetalsName { get; set; }

        public override string ToString()
        {
            return StateDetalsName;
        }
    }
}