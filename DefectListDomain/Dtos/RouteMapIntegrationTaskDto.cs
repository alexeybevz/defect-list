using System;

namespace DefectListDomain.Dtos
{
    public class RouteMapIntegrationTaskDto
    {
        public int Id { get; set; }
        public string SessionGuid { get; set; }
        public string RowGuid { get; set; }
        public string OrdersCode { get; set; }
        public int OrdersYear { get; set; }
        public int TargetWpId { get; set; }
        public int BomItemId { get; set; }
        public string BomItemIds { get; set; }
        public int ProductId { get; set; }
        public string Detals { get; set; }
        public string Detal { get; set; }
        public float Qty { get; set; }
        public string PmcontrUserName { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public string CreatedRouteMap { get; set; }
        public int CreatedRouteMapId { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsPrint { get; set; }
        public bool IsWorkNeed { get; set; }
        public string TypeProg { get; set; }
        public string Nompredgu { get; set; }
        public string TypeTask { get; set; }
        public string DocType { get; set; }
        public int DocId { get; set; }
    }
}