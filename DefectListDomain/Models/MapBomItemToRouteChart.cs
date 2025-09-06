using System;

namespace DefectListDomain.Models
{
    public class MapBomItemToRouteChart
    {
        public int BomItemId { get; set; }
        public int MkartaId { get; set; }
        public string RouteChart_Number { get; set; }
        public string Detal { get; set; }
        public int ProductId { get; set; }
        public float QtyLaunched { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}