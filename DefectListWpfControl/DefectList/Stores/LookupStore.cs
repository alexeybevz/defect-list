using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    // Универсальный Store для lookup-справочников.
    // Один экземпляр — один тип справочника (Kind).
    // Зарегистрировать в IoC как именованные привязки или через фабрику.
    public class LookupStore
    {
        private readonly IGetLookupItemsQuery _getQuery;
        private readonly ISaveLookupItemCommand _saveCommand;
        private readonly ICreateLookupItemCommand _createCommand;

        private readonly List<LookupItem> _items;
        public LookupItemKind Kind { get; }
        public IReadOnlyList<LookupItem> Items => _items;

        public event Action ItemsLoaded;
        public event Action<LookupItem> ItemSaved;   // переименован или деактивирован+создан
        public event Action<LookupItem> ItemCreated;

        public LookupStore(
            LookupItemKind kind,
            IGetLookupItemsQuery getQuery,
            ISaveLookupItemCommand saveCommand,
            ICreateLookupItemCommand createCommand)
        {
            Kind = kind;
            _getQuery = getQuery;
            _saveCommand = saveCommand;
            _createCommand = createCommand;
            _items = new List<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var result = await _getQuery.ExecuteAsync(Kind);
            _items.Clear();
            _items.AddRange(result);
            ItemsLoaded?.Invoke();
        }

        // Сохранить изменённое имя.
        // Если IsUsed=false → UPDATE, возвращает тот же Id.
        // Если IsUsed=true  → деактивирует + INSERT, возвращает новый Id.
        public async Task<LookupItem> SaveAsync(int id, string newName, bool isUsed, string updatedBy)
        {
            var newId = await _saveCommand.ExecuteAsync(Kind, id, newName, isUsed, updatedBy);

            // Перезагружаем список — IsUsed может поменяться у других записей
            await LoadAsync();

            var saved = _items.Find(x => x.Id == newId);
            ItemSaved?.Invoke(saved);
            return saved;
        }

        // Создать новый элемент справочника
        public async Task<LookupItem> CreateAsync(string name, string createdBy)
        {
            var newId = await _createCommand.ExecuteAsync(Kind, name, createdBy);

            await LoadAsync();

            var created = _items.Find(x => x.Id == newId);
            ItemCreated?.Invoke(created);
            return created;
        }
    }
}