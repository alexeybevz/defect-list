using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Dtos;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Services
{
    public class BomItemsLoader : DbConnectionPmControlRepositoryBase, IBomItemsLoader
    {
        private readonly IGetAllBomItemsByBomHeaderQuery _getAllBomItemsByBomHeaderQuery;
        private readonly IGetBomViewService _getBomViewService;
        private readonly IUpdateBomItemIdInMapToRouteChartCommand _updateBomItemIdInMapToRouteChartCommand;
        private readonly ISaveBomItemCommand _saveBomItemCommand;
        private readonly ICollapseBomItemNodeCommand _collapseBomItemNodeCommand;
        private readonly IBomItemsCloner _bomItemsCloner;
        private readonly IDeleteBomItemCommand _deleteBomItemCommand;
        private readonly ICreateBomItemCommand _createBomItemCommand;

        public BomItemsLoader(
            IDbConnectionFactory dbConnectionFactory,
            IGetAllBomItemsByBomHeaderQuery getAllBomItemsByBomHeaderQuery,
            IGetBomViewService getBomViewService,
            IUpdateBomItemIdInMapToRouteChartCommand updateBomItemIdInMapToRouteChartCommand,
            ISaveBomItemCommand saveBomItemCommand,
            ICollapseBomItemNodeCommand collapseBomItemNodeCommand,
            IBomItemsCloner bomItemsCloner,
            IDeleteBomItemCommand deleteBomItemCommand,
            ICreateBomItemCommand createBomItemCommand) : base(dbConnectionFactory)
        {
            _getAllBomItemsByBomHeaderQuery = getAllBomItemsByBomHeaderQuery;
            _getBomViewService = getBomViewService;
            _updateBomItemIdInMapToRouteChartCommand = updateBomItemIdInMapToRouteChartCommand;
            _saveBomItemCommand = saveBomItemCommand;
            _collapseBomItemNodeCommand = collapseBomItemNodeCommand;
            _bomItemsCloner = bomItemsCloner;
            _deleteBomItemCommand = deleteBomItemCommand;
            _createBomItemCommand = createBomItemCommand;
        }

        public async Task<int> Execute(
            List<BomItem> deleteBomItems,
            List<AsupBomComponentDto> insertNewBomItems,
            int bomId,
            int? parentId,
            float parentQty,
            float selfQty,
            string userLogin)
        {
            int rootBomItemId;
            BomItem newRootAssembly;
            List<BomItem> newAssemblyBom;

            using (var db = await CreateOpenConnectionAsync())
            {
                using (var transaction = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    if (deleteBomItems != null && deleteBomItems.Any())
                        await _deleteBomItemCommand.Execute(deleteBomItems, userLogin, db, transaction);

                    rootBomItemId = await _createBomItemCommand.Execute(bomId, insertNewBomItems, parentId, parentQty, selfQty, userLogin, db, transaction);

                    var newBomItems = await _getAllBomItemsByBomHeaderQuery.Execute(bomId, db, transaction);
                    var newBom = (await _getBomViewService.Execute(newBomItems)).ToList();

                    newRootAssembly = newBom.First(x => x.Id == rootBomItemId);
                    newAssemblyBom = newBom.Where(x => x.StructureNumber.StartsWith(newRootAssembly.StructureNumber)).ToList();
                    _bomItemsCloner.CloneOldBomToNewBom(deleteBomItems, newAssemblyBom);
                    newAssemblyBom.ForEach(x => x.MapBomItemToRouteCharts?.ForEach(y =>
                        _updateBomItemIdInMapToRouteChartCommand.Execute(y.BomItemId, x.Id, db, transaction)));

                    transaction.Commit();
                }
            }

            if (newRootAssembly.UzelFlag == 1)
                await _collapseBomItemNodeCommand.Execute(newRootAssembly.Id, newAssemblyBom);

            if (deleteBomItems != null && deleteBomItems.Any()) // Если не происходит первоначальная загрузка состава изделия или узла
                newAssemblyBom?.ForEach(x => _saveBomItemCommand.Execute(x, userLogin));

            return rootBomItemId;
        }
    }
}