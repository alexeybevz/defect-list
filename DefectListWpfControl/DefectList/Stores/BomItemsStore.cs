using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListBusinessLogic;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListDomain.Services;

namespace DefectListWpfControl.DefectList.Stores
{
    public class BomItemsStore
    {
        private readonly IGetAllBomItemsByBomHeaderQuery _getAllBomItemsByBomHeaderQuery;
        private readonly IGetBomViewService _getBomViewService;
        private readonly IGetAllBomItemLogsQuery _getAllBomItemLogsQuery;
        private readonly IBomItemsEditor _bomItemsEditor;
        private readonly List<BomItem> _bomItems;
        public IEnumerable<BomItem> BomItems => _bomItems;

        public event Action BomItemsLoaded;
        public event Action<BomItem> BomItemsLoadedById;
        public event Action<int> BomItemExpanded;
        public event Action<int> BomItemCollapsed;
        public event Action BomItemsAdded;
        public event Action BomItemsReplaced;
        public event Action BomItemsNameReplaced;
        public event Action BomItemsDeleted;

        public BomItemsStore(
            IGetAllBomItemsByBomHeaderQuery getAllBomItemsByBomHeaderQuery,
            IGetBomViewService getBomViewService,
            IGetAllBomItemLogsQuery getAllBomItemLogsQuery,
            IBomItemsEditor bomItemsEditor)
        {
            _getAllBomItemsByBomHeaderQuery = getAllBomItemsByBomHeaderQuery;
            _getBomViewService = getBomViewService;
            _getAllBomItemLogsQuery = getAllBomItemLogsQuery;
            _bomItemsEditor = bomItemsEditor;
            _bomItems = new List<BomItem>();

            _bomItemsEditor.OnError += (sender, args) =>
            {
                MessageBox.Show(args.Message);
            };
        }

        public async Task Load(int bomId)
        {
            var bomItems = await _getAllBomItemsByBomHeaderQuery.Execute(bomId);
            bomItems = await _getBomViewService.Execute(bomItems);

            _bomItems.Clear();
            _bomItems.AddRange(bomItems);

            BomItemsLoaded?.Invoke();
        }

        public async Task LoadById(int id)
        {
            var bomItem = await _getAllBomItemsByBomHeaderQuery.ExecuteById(id);

            int currentIndex = _bomItems.FindIndex(x => x.Id == bomItem.Id);
            if (currentIndex != -1)
            {
                bomItem.QtyMnf = _bomItems[currentIndex].QtyMnf;
                bomItem.StructureNumber = _bomItems[currentIndex].StructureNumber;

                _bomItems[currentIndex] = bomItem;
            }
            else
            {
                MessageBox.Show("Строка была удалена другим пользователем.\nДефектовочная ведомость будет обновлена целиком.");
                await Load(bomItem.BomId);
            }

            BomItemsLoadedById?.Invoke(bomItem);
        }

        public async Task Expand(int assemblyBomItemId, string assemblyBomItemStructureNumber, List<BomItem> assemblyBom, bool isExpandAll = false)
        {
            await _bomItemsEditor.Expand(assemblyBomItemId, assemblyBomItemStructureNumber, assemblyBom, isExpandAll);

            BomItemExpanded?.Invoke(assemblyBomItemId);
        }

        public async Task Collapse(int assemblyBomItemId, List<BomItem> assemblyBom)
        {
            await _bomItemsEditor.Collapse(assemblyBomItemId, assemblyBom);

            BomItemCollapsed?.Invoke(assemblyBomItemId);
        }

        public async void Add(BomHeader bomHeader, BomItem parentBomItem, BomItem addBomItem, string login)
        {
            await _bomItemsEditor.Add(bomHeader, parentBomItem, addBomItem, login);

            BomItemsAdded?.Invoke();
        }

        public async void Replace(BomHeader bomHeader, BomItem oldBomItem, BomItem newBomItem, List<BomItem> bom, string login)
        {
            await _bomItemsEditor.Replace(bomHeader, oldBomItem, newBomItem, bom, login);

            BomItemsReplaced?.Invoke();
        }

        public async void ReplaceName(BomHeader bomHeader, BomItem oldBomItem, string newDetal, string login)
        {
            await _bomItemsEditor.ReplaceName(bomHeader, oldBomItem, newDetal, login);

            BomItemsNameReplaced?.Invoke();
        }

        public async void Delete(BomHeader bomHeader, BomItem deletedBomItem, List<BomItem> deletedBomItems, string userLogin)
        {
            await _bomItemsEditor.Delete(bomHeader, deletedBomItem, deletedBomItems, userLogin);

            BomItemsDeleted?.Invoke();
        }

        public async Task<IEnumerable<BomItem>> GetBomItemIsShowedView(int bomId)
        {
            var bomItems = (await _getAllBomItemsByBomHeaderQuery.Execute(bomId)).IsShowed().AsEnumerable();
            bomItems = await _getBomViewService.Execute(bomItems);
            return bomItems;
        }

        public async Task<IEnumerable<BomItem>> GetBomItemIsDatabaseView(int bomId)
        {
            var bomItems = (await _getAllBomItemsByBomHeaderQuery.Execute(bomId)).AsEnumerable();
            bomItems = await _getBomViewService.Execute(bomItems);
            return bomItems;
        }

        public async Task<IEnumerable<BomItemLog>> GetBomItemLogs(int bomId)
        {
            return await _getAllBomItemLogsQuery.ExecuteByBomId(bomId);
        }

        public async Task<IEnumerable<BomItem>> GetBomItemsByProductId(int productId)
        {
            return (await _getAllBomItemsByBomHeaderQuery.ExecuteByProductId(productId)).IsShowed().ToList();
        }

        public async Task<BomItem> GetBomItemById(int bomItemId)
        {
            return await _getAllBomItemsByBomHeaderQuery.ExecuteById(bomItemId);
        }

        public async Task<IEnumerable<FinalDecisionChanging>> GetFinalDecisionChangings(DateTime startDate, DateTime endDate)
        {
            return await _getAllBomItemLogsQuery.FinalDecisionChangings(startDate, endDate);
        }
    }
}