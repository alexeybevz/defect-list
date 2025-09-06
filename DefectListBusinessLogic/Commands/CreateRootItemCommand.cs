using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateRootItemCommand : DbConnectionPmControlRepositoryBase, ICreateRootItemCommand
    {
        public CreateRootItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(RootItem rootItem)
        {
            var parameters = new
            {
                Izdels = rootItem.Izdels,
                Izdel = rootItem.Izdel,
                IzdelInitial = rootItem.IzdelInitial,
                IzdelIma = rootItem.IzdelIma,
                IzdelTyp = rootItem.IzdelTyp
            };

            using (var db = await CreateOpenConnectionAsync())
            {
                var query = @"INSERT INTO RootItem (Izdels, Izdel, IzdelInitial, IzdelIma, IzdelTyp)
                              VALUES(@Izdels, @Izdel, @IzdelInitial, @IzdelIma, @IzdelTyp);
                              SELECT SCOPE_IDENTITY()";

                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    rootItem.Id = await db.QuerySingleAsync<int>(query, parameters, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}