using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    public class RootItemsStore
    {
        private readonly IGetAllRootItemsQuery _getAllRootItemsQuery;
        private readonly ICreateRootItemCommand _createRootItemCommand;
        private readonly IDeleteRootItemCommand _deleteRootItemCommand;

        private readonly List<RootItem> _rootItems;

        public IEnumerable<RootItem> RootItems => _rootItems;

        public event Action RootItemLoaded;
        public event Action<RootItem> RootItemAdded;
        public event Action<int> RootItemDeleted;

        public RootItemsStore(
            IGetAllRootItemsQuery getAllRootItemsQuery,
            ICreateRootItemCommand createRootItemCommand,
            IDeleteRootItemCommand deleteRootItemCommand)
        {
            _getAllRootItemsQuery = getAllRootItemsQuery;
            _createRootItemCommand = createRootItemCommand;
            _deleteRootItemCommand = deleteRootItemCommand;

            _rootItems = new List<RootItem>();
        }

        public async Task Load()
        {
            var rootItems = await _getAllRootItemsQuery.Execute();

            _rootItems.Clear();
            _rootItems.AddRange(rootItems.OrderBy(x => x.Izdel).ToList());

            RootItemLoaded?.Invoke();
        }

        public async Task Add(RootItem rootItem)
        {
            await _createRootItemCommand.Execute(rootItem);

            _rootItems.Add(rootItem);

            RootItemAdded?.Invoke(rootItem);
        }

        public async Task Delete(int id)
        {
            await _deleteRootItemCommand.Execute(id);

            _rootItems.RemoveAll(x => x.Id == id);

            RootItemDeleted?.Invoke(id);
        }
    }
}