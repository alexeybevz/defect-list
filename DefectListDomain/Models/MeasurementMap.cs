using System;
using System.Collections.Generic;

namespace DefectListDomain.Models
{
    public class MeasurementMap
    {
        public int Id { get; set; }
        public int BomItemId { get; set; }
        public int MeasurementMapDictionaryId { get; set; }
        public int MeasurementsMapTypeFormId { get; set; }
        public string MeasurementsMapTypeFormName { get; set; }
        public byte Status { get; set; }
        public string Comment { get; set; }
        public List<string> SketchFilePaths { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
        public IReadOnlyCollection<MeasurementMapItem> Items { get; set; }
    }
}