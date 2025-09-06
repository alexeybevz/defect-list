using System.Collections.Generic;
using System.Configuration;
using Ninject.Modules;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListDemoWpfApp
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            var connections = new Dictionary<DatabaseConnectionName, ConnectionStringSettings>
            {
                {DatabaseConnectionName.PmControl, ConfigurationManager.ConnectionStrings["PMcontrol_Product"]},
            };

            Bind<IDictionary<DatabaseConnectionName, ConnectionStringSettings>>().ToConstant(connections)
                .InSingletonScope();
            Bind<IDbConnectionFactory>().To<DapperDbConnectionFactory>().InSingletonScope();
        }
    }
}