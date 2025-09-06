using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Stores
{
    public static class BomItemsConstantsStore
    {
        static BomItemsConstantsStore()
        {
            DecisionsToCreateRouteMap = new[] { "ремонт", "использовать", "заменить", "скомплектовать" };

            IsRequiredSubmitDict = new Dictionary<IsBomItemRequiredSubmit, string>()
            {
                { IsBomItemRequiredSubmit.Yes, "Требуется предъявить ВП" },
                { IsBomItemRequiredSubmit.No, "Не требует предъявления ВП" }
            };

            IsSubmittedDict = new Dictionary<IsBomItemSubmitted, string>()
            {
                { IsBomItemSubmitted.Yes, "Предъявлено ВП" },
                { IsBomItemSubmitted.No, "Не предъявлено ВП" }
            };

            FilteredColumns = new Dictionary<string, BomItemsFiltersStore.IBomItemFilter>
            {
                { "По всем столбцам", null },
                { "Структурный номер", new BomItemsFiltersStore.StructureNumberFilter() },
                { "Обозначение", new BomItemsFiltersStore.DetalFilter() },
                { "Наименование", new BomItemsFiltersStore.DetalImaFilter() },
                { "Заводской номер", new BomItemsFiltersStore.SerialNumberFilter() },
                { "Требуется предъявить ВП", new BomItemsFiltersStore.IsRequiredSubmitTextFilter() },
                { "Предъявлено ВП", new BomItemsFiltersStore.IsSubmittedTextFilter() },
                { "Рекомендация РКД", new BomItemsFiltersStore.RepairMethodNameFilter() },
                { "Первоначальное решение", new BomItemsFiltersStore.InitialDecisionFilter() },
                { "Окончательное решение", new BomItemsFiltersStore.FinalDecisionFilter() },
            };
        }

        /// <summary>
        /// Решения доступные для создания маршрутных карт и комплектования
        /// </summary>
        public static IEnumerable<string> DecisionsToCreateRouteMap { get; }

        public static Dictionary<IsBomItemRequiredSubmit, string> IsRequiredSubmitDict { get; }
        public static Dictionary<IsBomItemSubmitted, string> IsSubmittedDict { get; }

        public static Dictionary<string, BomItemsFiltersStore.IBomItemFilter> FilteredColumns { get; }

        /// <summary>
        /// Количество дней, в течение которых разрешено создание МК.
        /// </summary>
        public const int CountDaysAllowedCreateRouteCharts = 5;
    }
}