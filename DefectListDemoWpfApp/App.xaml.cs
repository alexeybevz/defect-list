using System;
using System.Collections.Generic;
using System.Windows;
using DefectListWpfControl;
using ReporterDomain.Auth;

namespace DefectListDemoWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Create a custom principal with an anonymous identity at startup
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            IocKernel.Initialize(
                new IocConfiguration());
            DefectListIocKernel.Initialize(new DefectListNinjectModule());

            SetDebugCustomPrincipalIdentity(customPrincipal);
            Application.Current.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

            base.OnStartup(e);
        }

        private static void SetDebugCustomPrincipalIdentity(CustomPrincipal customPrincipal)
        {
            var ldapUser = new LdapUser()
            {
                CN = "admin",
                Name = "admin",
                Roles = new List<string>() { "Администраторы домена" }
            };

            customPrincipal.Identity = new CustomIdentity(ldapUser, 1, new List<Role>() { new Role() { RoleId = 1, RoleName = "Администратор" } });
        }
    }
}