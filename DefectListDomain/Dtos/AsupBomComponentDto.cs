using System;

namespace DefectListDomain.Dtos
{
    public class AsupBomComponentDto
    {
        public string uzels { get; set; }
        public string uzel { get; set; }
        public string UzelCodeSL { get; set; }
        public string detals { get; set; }
        public string detal { get; set; }
        public string detalInitial { get; set; }
        public string DetalCodeSL { get; set; }
        public string imadetal { get; set; }
        public float plan_kol { get; set; }
        public string units { get; set; }
        public string typ { get; set; }
        public string Comment { get; set; }
        public int OrderByPriority
        {
            get
            {
                switch (typ)
                {
                    case "издел": return 0;
                    case "матер": return 2;
                    case "сб.ед": return 3;
                    default: return 1;
                }
            }
        }
        public byte UzelFlag => (byte)(typ == "сб.ед" || typ == "издел" ? 1 : 0);
        public bool IsExistsChilds { get; set; }
        public bool IsPki => typ == "покуп";
        public bool IsOtlivka => typ == "литье";
        public bool IsDetal => typ == "дет";
        public string StructureNumber { get; set; }
        public string StructureNumberParent { get; set; }
        public string StructureName { get; set; }
        public int? MinOptimalRunBatch { get; set; }
        public int? Uzel_Product_Id { get; set; }
        public int? Uzel_Code_LSF82 { get; set; }
        public int? Detal_Product_Id { get; set; }
        public int? Detal_Code_LSF82 { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}