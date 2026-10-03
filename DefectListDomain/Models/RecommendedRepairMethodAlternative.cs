namespace DefectListDomain.Models
{
    // Один допустимый вариант метода ремонта для строки справочника карты измерения.
    // Используется для построения выпадающего списка на форме редактирования
    // экземпляра карты измерения.
    public class RecommendedRepairMethodAlternative
    {
        public int RecommendedRepairMethodId { get; set; }
        public string RecommendedRepairMethodName { get; set; }
    }
}