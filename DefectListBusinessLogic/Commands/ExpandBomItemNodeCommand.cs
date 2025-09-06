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
    public class ExpandBomItemNodeCommand : DbConnectionPmControlRepositoryBase, IExpandBomItemNodeCommand
    {
        public ExpandBomItemNodeCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int assemblyBomItemId, string assemblyBomItemStructureNumber, List<BomItem> assemblyBom, bool isExpandAll = false)
        {
            var bomWithoutRoot = assemblyBom.Where(x => isExpandAll ? x.StructureNumber.StartsWith(assemblyBomItemStructureNumber) && x.Id != assemblyBomItemId : x.ParentId == assemblyBomItemId).ToList();

            using (var db = await CreateOpenConnectionAsync())
            {
                using (var tran = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var query = "UPDATE BomItem SET IsExpanded = @value WHERE Id = @Id";
                    await db.ExecuteAsync(query, new { value = true, Id = assemblyBomItemId }, tran);

                    bomWithoutRoot.ForEach(x =>
                    {
                        query = "UPDATE BomItem SET IsShowItem = @value WHERE Id = @Id";
                        db.Execute(query, new { value = true, x.Id }, tran);

                        if (x.UzelFlag == 1)
                        {
                            query = "UPDATE BomItem SET IsExpanded = @value WHERE Id = @Id";
                            db.Execute(query, new { value = isExpandAll, x.Id }, tran);
                        }
                    });

                    tran.Commit();
                }
            }
        }
    }
}