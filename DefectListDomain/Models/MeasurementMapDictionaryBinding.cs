namespace DefectListDomain.Models
{
    public class MeasurementMapDictionaryBinding
    {
        public int Id { get; set; }
        public int MeasurementMapDictionaryId { get; set; }
        public int RootItemId { get; set; }

        // Izdel из RootItem — для отображения в UI и записи в лог
        public string RootItemName { get; set; }

        public int Code_LSF82 { get; set; }
    }
}