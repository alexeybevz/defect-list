using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Stores
{
    public static class PermissionsStore
    {
        private static readonly IEnumerable<string> Admins;
        private static readonly IEnumerable<string> SuperUsers;
        private static readonly IEnumerable<string> PassiveUsers;
        private static readonly IEnumerable<string> WriteAccessUsers;
        private static readonly IEnumerable<string> CanCreateRouteMapsUsers;
        private static readonly IEnumerable<string> CanUnrestrictedCreateRouteMapsUsers;
        private static readonly IEnumerable<string> CanOtkCreateRouteMapsUsers;
        private static readonly IEnumerable<string> CanPdoCreateRouteMapsUsers;
        private static readonly IEnumerable<string> CanFillFinalDecisionBasedOnPmControlUsers;

        static PermissionsStore()
        {
            Admins = new List<string>()
            {
                "Администратор",
                "ОИТ"
            };

            SuperUsers = new List<string>(Admins)
            {
                "ОТК Деф.вед. руководство"
            };

            PassiveUsers = new List<string>()
            {
                "ОТК Деф.вед. ВП"
            };

            WriteAccessUsers = new List<string>(SuperUsers)
            {
                "ОТК Деф.вед. запись"
            };

            CanFillFinalDecisionBasedOnPmControlUsers = new List<string>(SuperUsers)
            {
                "ОТК Деф.вед. заполнение окончательного решения из Контроля"
            };

            var otkCreateRouteMap = "ОТК Деф.вед. создание МК";
            var otkUnrestrictedCreateRouteMap = "ОТК Деф.вед. создание МК без ограничений";
            var pdoUnrestrictedCreateRouteMap = "ПДО Деф.вед. создание МК без ограничений";

            CanCreateRouteMapsUsers = new List<string>(SuperUsers)
            {
                otkCreateRouteMap,
                otkUnrestrictedCreateRouteMap,
                pdoUnrestrictedCreateRouteMap
            };
            CanUnrestrictedCreateRouteMapsUsers = new List<string>(Admins)
            {
                otkUnrestrictedCreateRouteMap,
                pdoUnrestrictedCreateRouteMap
            };

            CanOtkCreateRouteMapsUsers = new List<string>(SuperUsers)
            {
                otkCreateRouteMap,
                otkUnrestrictedCreateRouteMap,
            };

            CanPdoCreateRouteMapsUsers = new List<string>(Admins)
            {
                pdoUnrestrictedCreateRouteMap
            };

            FieldPermissionToRoles = new List<FieldPermissionToRole>()
            {
                new FieldPermissionToRole() { Role = "Администратор", Rank = 1, FieldPermissions = BomItemFieldPermissionToRoleStore.AdminFieldsPermissionSet},
                new FieldPermissionToRole() { Role = "ОИТ", Rank = 2, FieldPermissions = BomItemFieldPermissionToRoleStore.AdminFieldsPermissionSet},
                new FieldPermissionToRole() { Role = "ОТК Деф.вед. руководство", Rank = 3, FieldPermissions = BomItemFieldPermissionToRoleStore.WriteFieldsPermissionSet},
                new FieldPermissionToRole() { Role = "ОТК Деф.вед. запись", Rank = 4, FieldPermissions = BomItemFieldPermissionToRoleStore.WriteFieldsPermissionSet},
                new FieldPermissionToRole() { Role = "ОТК Деф.вед. чтение", Rank = 5, FieldPermissions = BomItemFieldPermissionToRoleStore.ReadOnyFieldsPermissionSet},

            }.GroupBy(x => x.Role).ToDictionary(k => k.Key, v => v.FirstOrDefault());
        }

        public static Dictionary<string, FieldPermissionToRole> FieldPermissionToRoles { get; set; }

        public static bool IsSuperUser => UserIsInRole(SuperUsers);
        public static bool IsPassiveUser => UserIsInRole(PassiveUsers);
        public static bool IsAdmin => UserIsInRole(Admins);
        public static bool IsWriteAccessUser => UserIsInRole(WriteAccessUsers);
        public static bool IsCanCreateRouteMapsUser => UserIsInRole(CanCreateRouteMapsUsers);
        public static bool IsCanUnrestrictedCreateRouteMapsUser => UserIsInRole(CanUnrestrictedCreateRouteMapsUsers);
        public static bool IsCanFillFinalDecisionBasedOnPmControlUser => UserIsInRole(CanFillFinalDecisionBasedOnPmControlUsers);
        public static bool IsCanOtkCreateRouteMapsUser => UserIsInRole(CanOtkCreateRouteMapsUsers);
        public static bool IsCanPdoCreateRouteMapsUser => UserIsInRole(CanPdoCreateRouteMapsUsers);

        private static bool UserIsInRole(IEnumerable<string> users)
        {
            var user = Thread.CurrentPrincipal as CustomPrincipal;
            var result = users.Any(role => user.IsInRole(role));
            return result;
        }
    }
}