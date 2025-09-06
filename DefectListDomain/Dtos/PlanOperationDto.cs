namespace DefectListDomain.Dtos
{
    public class PlanOperationDto
    {
        public string Detals { get; set; }
        public string Detal { get; set; }
        public string ImaDetal { get; set; }
        public string Ceh { get; set; }
        public string Uch { get; set; }
        public string Dept { get; set; }
        public int Nomoper { get; set; }
        public string Imaoper { get; set; }
        public int Razr { get; set; }
        public float Stavka { get; set; }
        public float Tpz_on_one_det { get; set; }
        public float Top_on_one_det { get; set; }
        public string Type_Work { get; set; }
        public string Kind_Work { get; set; }
        public string Kind_Work_Name
        {
            get
            {
                switch (Kind_Work)
                {
                    case "С": return "Сдельная";
                    case "П": return "Повременная";
                    default: return "Без оплаты труда";
                }
            }
        }
        public int MinOptimalRunBatch { get; set; }
    }
}