using System;

namespace DefectListDomain.Models
{
    // Готовые к показу значения строки карты измерения.
    // Единый источник итоговых данных, которые должны отобразить в Excel и форме для read-only просмотра
    // Форма редактирования карты измерения этой моделью не пользуется -
    // там нужны сырые значения для правки, а не отображаемые.
    public class MeasurementMapItemDisplayModel
    {
        /// <summary>
        ///  Возможный дефект. В начале строки содержит кастомную нумерацию
        /// </summary>
        public string PossibleDefectName { get; set; }
        /// <summary>
        /// Предельное или номинальное значение
        /// </summary>
        public string NominalValueName { get; set; }
        /// <summary>
        /// Измеренное/фактическое значение
        /// </summary>
        public string ActualValue { get; set; }
        /// <summary>
        /// Дата последнего изменения измеренного значения
        /// </summary>
        public DateTime? ActualValueRecordDate { get; set; }
        /// <summary>
        /// Заключение и рекомендуемые методы ремонта
        /// </summary>
        public string ActualRecommendedRepairMethodName { get; set; }
        /// <summary>
        /// Отметка о выполнении работ по устранению дефекта
        /// </summary>
        public string MarkOfWorkCompletion { get; set; }
        /// <summary>
        /// Требование после ремонта
        /// </summary>
        public string RequirementPostRepairName { get; set; }
    }
}