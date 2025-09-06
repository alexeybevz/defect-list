using System;
using System.Linq;
using System.Threading.Tasks;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListDomain.Services;

namespace DefectListBusinessLogic.Commands
{
    public class LoadBomItemsToBomHeaderCommand : DbConnectionPmControlRepositoryBase, ILoadBomItemsToBomHeaderCommand
    {
        private readonly IGetAllBomItemsByBomHeaderQuery _getAllBomItemsByBomHeaderQuery;
        private readonly IGetBomViewService _bomViewService;
        private readonly IBomItemsLoader _bomItemsLoader;
        private readonly IAsupBomContextFactory _asupBomContextFactory;

        public LoadBomItemsToBomHeaderCommand(
            IDbConnectionFactory dbConnectionFactory,
            IAsupBomContextFactory asupBomContextFactory,
            IGetAllBomItemsByBomHeaderQuery getAllBomItemsByBomHeaderQuery,
            IGetBomViewService bomViewService,
            IBomItemsLoader bomItemsLoader) : base(dbConnectionFactory)
        {
            _asupBomContextFactory = asupBomContextFactory;
            _getAllBomItemsByBomHeaderQuery = getAllBomItemsByBomHeaderQuery;
            _bomViewService = bomViewService;
            _bomItemsLoader = bomItemsLoader;
        }

        public async Task Execute(BomHeader bomHeader, string userLogin)
        {
            var newBom = _asupBomContextFactory.AskBase(bomHeader.RootItem.Izdel, 1, bomHeader.DateOfSpecif.AddDays(1));

            if (newBom.Count == 0)
                throw new Exception($"Отсутствует состав на ДСЕ {bomHeader.RootItem.Izdel} на дату {bomHeader.DateOfSpecif.AddDays(1):dd.MM.yyyy}");

            var oldBomItems = (await _getAllBomItemsByBomHeaderQuery.Execute(bomHeader.BomId)).ToList();
            var oldBom = (await _bomViewService.Execute(oldBomItems)).ToList();

            await _bomItemsLoader.Execute(oldBom, newBom, bomHeader.BomId, null, 1, 1, userLogin);
        }
    }
}