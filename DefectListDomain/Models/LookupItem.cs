namespace DefectListDomain.Models
{
    // Универсальная модель для справочных таблиц:
    // PossibleDefect, NominalValue, RecommendedRepairMethod, RequirementPostRepair.
    // Все они имеют одинаковую структуру: Id, Name, IsActive.
    public class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        // Используется ли запись хотя бы в одной строке MeasurementMapDictionaryItem
        // или MeasurementMapItem. Заполняется запросом при загрузке.
        // True = нельзя переименовать, только деактивировать + создать новую.
        public bool IsUsed { get; set; }
    }
}