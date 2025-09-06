namespace DefectListDomain.Models
{
    public class RootItem
    {
        public int Id { get; set; }
        public string Izdels { get; set; }
        public string Izdel { get; set; }
        public string IzdelInitial { get; set; }
        public string IzdelIma { get; set; }
        public string IzdelTyp { get; set; }

        public override string ToString()
        {
            return Izdel;
        }
    }
}