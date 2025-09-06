using System.Collections.Generic;
using System.Linq;

namespace DefectListWpfControl.DefectList.Stores
{
    public static class BomItemFieldPermissionToRoleStore
    {
        static BomItemFieldPermissionToRoleStore()
        {
            ReadOnyFieldsPermissionSet = new List<FieldPermission>()
            {
                new FieldPermission() { FieldName = "StructureNumber", FieldLabel = "Структура", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "QtyRestore", FieldLabel = "Кол-во ремонт", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "QtyReplace", FieldLabel = "Кол-во замена", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "Defect", FieldLabel = "Установленный дефект", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "DefectToDecisionMaps", FieldLabel = "Возможные дефекты", CanRead = false, CanEdit = false },
                new FieldPermission() { FieldName = "Decision", FieldLabel = "Решение по устранению", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "FinalDecision", FieldLabel = "Окончательное решение по устранению", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "AvailableTechnologicalProcess", FieldLabel = "Доступные тех.процессы", CanRead = false, CanEdit = false },
                new FieldPermission() { FieldName = "TechnologicalProcessUsed", FieldLabel = "Применяемый тех. процесс", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "IsRequiredSubmit", FieldLabel = "Требуется предъявить ВП", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "IsSubmitted", FieldLabel = "Предъявлено ВП", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "CommentDef", FieldLabel = "Примечание", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "SerialNumber", FieldLabel = "Заводской номер", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "RepairMethodName", FieldLabel = "Рекомендация РКД", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "ResearchAction", FieldLabel = "Мероприятия по изучению", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "ResearchResult", FieldLabel = "Результаты изучения", CanRead = true, CanEdit = false },
                new FieldPermission() { FieldName = "SaveButtons", FieldLabel = "Кнопки сохранения", CanRead = false, CanEdit = false },
                new FieldPermission() { FieldName = "CreateOneRouteChartCommandButton", FieldLabel = "Создать и распечатать МК", CanRead = false, CanEdit = false },
            }.GroupBy(x => x.FieldName).ToDictionary(k => k.Key, v => v.FirstOrDefault());

            WriteFieldsPermissionSet = new List<FieldPermission>()
            {
                new FieldPermission() { FieldName = "StructureNumber", FieldLabel = "Структура", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "QtyRestore", FieldLabel = "Кол-во ремонт", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "QtyReplace", FieldLabel = "Кол-во замена", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "Defect", FieldLabel = "Установленный дефект", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "DefectToDecisionMaps", FieldLabel = "Возможные дефекты", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "Decision", FieldLabel = "Решение по устранению", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "FinalDecision", FieldLabel = "Окончательное решение по устранению", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "AvailableTechnologicalProcess", FieldLabel = "Доступные тех.процессы", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "TechnologicalProcessUsed", FieldLabel = "Применяемый тех. процесс", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "IsRequiredSubmit", FieldLabel = "Требуется предъявить ВП", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "IsSubmitted", FieldLabel = "Предъявлено ВП", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "CommentDef", FieldLabel = "Примечание", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "SerialNumber", FieldLabel = "Заводской номер", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "RepairMethodName", FieldLabel = "Рекомендация РКД", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "ResearchAction", FieldLabel = "Мероприятия по изучению", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "ResearchResult", FieldLabel = "Результаты изучения", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "SaveButtons", FieldLabel = "Кнопки сохранения", CanRead = true, CanEdit = true },
                new FieldPermission() { FieldName = "CreateOneRouteChartCommandButton", FieldLabel = "Создать и распечатать МК", CanRead = true, CanEdit = true },
            }.GroupBy(x => x.FieldName).ToDictionary(k => k.Key, v => v.FirstOrDefault());

            AdminFieldsPermissionSet = new List<FieldPermission>(WriteFieldsPermissionSet.Select(x => x.Value).ToList())
            {
                new FieldPermission() { FieldName = "Id", FieldLabel = "Id", CanRead = true, CanEdit = false },
            }.GroupBy(x => x.FieldName).ToDictionary(k => k.Key, v => v.FirstOrDefault());
        }

        public static Dictionary<string, FieldPermission> AdminFieldsPermissionSet { get; set; }

        public static Dictionary<string, FieldPermission> WriteFieldsPermissionSet { get; set; }

        public static Dictionary<string, FieldPermission> ReadOnyFieldsPermissionSet { get; set; }
    }

    public class FieldPermission
    {
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public bool CanRead { get; set; }
        public bool CanEdit { get; set; }
    }

    public class FieldPermissionToRole
    {
        public string Role { get; set; }
        public int Rank { get; set; }
        public Dictionary<string, FieldPermission> FieldPermissions { get; set; }
    }
}