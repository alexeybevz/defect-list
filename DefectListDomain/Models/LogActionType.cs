using System;

namespace DefectListDomain.Models
{
    public class LogActionType
    {
        public int LogActionTypeId { get; set; }
        public string LogActionTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
