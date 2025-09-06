namespace DefectListDomain.Dtos
{
    public class ProductModeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public byte ProductState { get; set; }
        public int? AccountCostId { get; set; }
        public string AccountCostCode { get; set; }
        public int? KodWasteId { get; set; }
        public string KodWasteName { get; set; }
    }
}