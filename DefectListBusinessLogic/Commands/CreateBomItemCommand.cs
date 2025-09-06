using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Dtos;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateBomItemCommand : DbConnectionPmControlRepositoryBase, ICreateBomItemCommand
    {
        public CreateBomItemCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<int> Execute(int bomId, ICollection<AsupBomComponentDto> bomToLoad, int? parentItemId, float parentQty, float selfQty, string userLogin)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var root = bomToLoad.FirstOrDefault(x => x.StructureNumber == "1");
                var rootBomItemId =
                    parentItemId == null
                        ? InsertBomItem(bomId, parentItemId, 1, root, userLogin, db, null)
                        : InsertBomItem(bomId, parentItemId, parentQty, root, userLogin, db, null);

                LoadBomRecursive(bomId, bomToLoad, rootBomItemId, "1", selfQty, userLogin, db, null);

                return rootBomItemId;
            }
        }

        public async Task<int> Execute(int bomId, ICollection<AsupBomComponentDto> bomToLoad, int? parentItemId, float parentQty, float selfQty, string userLogin,
            IDbConnection db, IDbTransaction transaction)
        {
            var root = bomToLoad.FirstOrDefault(x => x.StructureNumber == "1");
            var rootBomItemId =
                parentItemId == null
                    ? InsertBomItem(bomId, parentItemId, 1, root, userLogin, db, transaction)
                    : InsertBomItem(bomId, parentItemId, parentQty, root, userLogin, db, transaction);

            LoadBomRecursive(bomId, bomToLoad, rootBomItemId, "1", selfQty, userLogin, db, transaction);

            return rootBomItemId;
        }

        private void LoadBomRecursive(int bomId, ICollection<AsupBomComponentDto> bomToLoad, int? id, string structureNumber, float qty, string userLogin, IDbConnection db, IDbTransaction transaction)
        {
            var pattern = $"^{structureNumber.Replace(" ", string.Empty).Replace(".", "\\.")}\\.[0-9]+$";

            var childs = bomToLoad.Where(x => Regex.IsMatch(x.StructureNumber.Replace(" ", string.Empty),
                pattern, RegexOptions.IgnoreCase)).ToList();
            foreach (var obj in childs)
            {
                var bomItemId = InsertBomItem(bomId, id, qty, obj, userLogin, db, transaction);

                LoadBomRecursive(bomId, bomToLoad, bomItemId, obj.StructureNumber, obj.plan_kol, userLogin, db, transaction);
            }
        }

        private int InsertBomItem(int bomId, int? parentId, float parentQty, AsupBomComponentDto obj, string userLogin, IDbConnection db, IDbTransaction transaction)
        {
            var bomItem = new BomItem()
            {
                BomId = bomId,
                ParentId = parentId,
                Detals = obj.detals,
                Detal = obj.detal,
                DetalIma = obj.imadetal,
                DetalTyp = obj.typ,
                DetalUm = obj.units,
                QtyMnf = obj.plan_kol / parentQty,  // На вход приходит состав с перемноженным кол-вом на ветках, в бд загружается состав на единицу, поэтому происходит деление на кол-во в родительском узле
                Comment = obj.Comment,
                CreatedBy = userLogin,
                UpdatedBy = userLogin,
                IsExpanded = obj.UzelFlag == 1 ? true : (bool?)null,
                IsShowItem = true,
                ClassifierID = obj.DetalCodeSL,
                ProductID = obj.Detal_Product_Id,
                Code_LSF82 = obj.Detal_Code_LSF82,
            };

            var bomItemId = db.QuerySingle<int>(GetInsertBomItemquery(), bomItem, transaction);
            return bomItemId;
        }

        private string GetInsertBomItemquery()
        {
            return @"INSERT INTO dbo.BomItem (BomId, ParentId, Detals, Detal, DetalIma, DetalTyp, DetalUm, QtyMnf, Comment, CreatedBy, UpdatedBy, IsExpanded, IsShowItem, ClassifierID, ProductID, Code_LSF82)
                     VALUES (@BomId, @ParentId, @Detals, @Detal, @DetalIma, @DetalTyp, @DetalUm, @QtyMnf, @Comment, @CreatedBy, @UpdatedBy, @IsExpanded, @IsShowItem, @ClassifierID, @ProductID, @Code_LSF82);
                     SELECT SCOPE_IDENTITY()";
        }
    }
}