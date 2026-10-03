using System;
using System.Collections.Generic;
using System.Linq;

namespace DefectListDomain.Models
{
    public class MeasurementMapDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code_LSF82 { get; set; }
        public int MeasurementsMapTypeFormId { get; set; }
        public string MeasurementsMapTypeFormName { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime RecordDate { get; set; }
        public string UpdatedBy { get; set; }
        public IEnumerable<MeasurementMapDictionaryItem> Items { get; set; }

        // Привязки к изделиям. Заполняются при загрузке через
        // GetAllMeasurementMapDictionariesQuery (метод AttachBindingsAsync).
        // При загрузке через GetMeasurementMapDictionaryByCodeQuery
        // не нужны — там выборка уже по конкретному изделию.
        public IReadOnlyList<MeasurementMapDictionaryBinding> Bindings { get; set; }

        // Готовая строка для отображения в таблице списка справочников.
        // Вычисляется на C# стороне из Bindings — не тянуть FOR XML PATH из SQL.
        public string BoundRootItemNames =>
            Bindings != null && Bindings.Count > 0
                ? string.Join("; ", Bindings.Select(b => b.RootItemName).ToList())
                : string.Empty;
    }
}