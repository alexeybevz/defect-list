namespace DefectListDomain.Dtos
{
    public class AuxiliaryMaterialDto
    {
        public int Id { get; set; }
        public int ParentProductId { get; set; }
        public int ParentCodeLsf82 { get; set; }
        public string ParentItem { get; set; }
        public string ParentUnionName { get; set; }
        public string ParentTyp { get; set; }
        public int ProductId { get; set; }
        public int CodeLsf82 { get; set; }
        public string Item { get; set; }
        public string UnionName { get; set; }
        public decimal QtyOnUnit { get; set; }
        public string Um { get; set; }
        public int WpId { get; set; }
        public string WpCeh { get; set; }
        public string CostAccoutingCode { get; set; }
        public string CostAccoutingCategory { get; set; }
    }
}