using System.Collections.Generic;

namespace DefectListDomain.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Detals { get; set; }
        public string Name { get; set; }
        public string ExtName { get; set; }
        public string Type { get; set; }
        public string Specification { get; set; }
        public string Um { get; set; }
        public string CodeErp { get; set; }
        public int CodeLsf82 { get; set; }
        public string MaterialLabel { get; set; }
        public string MaterialSubstitute { get; set; }
        public bool IsIntegralPart { get; set; }
        public bool IsAssembly { get; set; }
        public List<ProductModeDto> ProductModes { get; set; }
    }
}