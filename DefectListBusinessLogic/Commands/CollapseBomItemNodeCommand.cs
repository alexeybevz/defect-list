using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CollapseBomItemNodeCommand : DbConnectionPmControlRepositoryBase, ICollapseBomItemNodeCommand
    {
        public CollapseBomItemNodeCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int assemblyBomItemId, List<BomItem> assemblyBom)
        {
            var bomWithoutRoot = assemblyBom.Skip(1).ToList();

            using (var db = await CreateOpenConnectionAsync())
            {
                using (var tran = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var query = "UPDATE BomItem SET IsExpanded = @value WHERE Id = @Id";
                    await db.ExecuteAsync(query, new { value = false, Id = assemblyBomItemId }, tran);

                    bomWithoutRoot.ForEach(x =>
                    {
                        query = "UPDATE BomItem SET IsExpanded = null, IsShowItem = @value WHERE Id = @Id";
                        db.Execute(query, new { value = false, x.Id }, tran);
                    });
                    tran.Commit();
                }
            }
        }
    }
}