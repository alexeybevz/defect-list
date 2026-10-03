using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    // Store для списковой формы и формы строк справочника карт измерений.
    // Хранит список шапок (без строк) и текущий открытый справочник со строками.
    public class MeasurementMapDictionaryStore
    {
        private readonly IGetAllMeasurementMapDictionariesQuery _getAllQuery;
        private readonly IGetMeasurementMapDictionaryByIdQuery _getByIdQuery;
        private readonly ICreateMeasurementMapDictionaryCommand _createCommand;
        private readonly IArchiveMeasurementMapDictionaryCommand _archiveCommand;
        private readonly ICreateMeasurementMapDictionaryVersionCommand _createVersionCommand;
        private readonly ICopyMeasurementMapDictionaryCommand _copyCommand;
        private readonly ICreateMeasurementMapDictionaryItemCommand _createItemCommand;
        private readonly IUpdateMeasurementMapDictionaryItemCommand _updateItemCommand;
        private readonly IDeleteMeasurementMapDictionaryItemCommand _deleteItemCommand;
        private readonly IGetAllRootItemsQuery _getAllRootItemsQuery;

        private readonly List<MeasurementMapDictionary> _dictionaries;
        private readonly List<RootItem> _rootItems;

        public IReadOnlyList<MeasurementMapDictionary> Dictionaries => _dictionaries;

        // Список всех изделий — передаётся в ViewModel формы создания
        // для отображения галочек. Грузится один раз при первом обращении.
        public IReadOnlyList<RootItem> RootItems => _rootItems;

        // Текущий открытый справочник (со строками). Null = ни один не открыт.
        public MeasurementMapDictionary CurrentDictionary { get; private set; }

        // --- События ---
        public event Action DictionariesLoaded;
        public event Action<MeasurementMapDictionary> DictionaryCreated;
        public event Action<MeasurementMapDictionary> DictionaryCopied;
        public event Action<int> DictionaryArchived;
        public event Action<MeasurementMapDictionary> DictionaryVersionCreated;
        public event Action CurrentDictionaryLoaded;
        public event Action<MeasurementMapDictionaryItem> DictionaryItemCreated;
        public event Action<MeasurementMapDictionaryItem> DictionaryItemUpdated;
        public event Action<int> DictionaryItemDeleted;

        public MeasurementMapDictionaryStore(
            IGetAllMeasurementMapDictionariesQuery getAllQuery,
            IGetMeasurementMapDictionaryByIdQuery getByIdQuery,
            ICreateMeasurementMapDictionaryCommand createCommand,
            IArchiveMeasurementMapDictionaryCommand archiveCommand,
            ICreateMeasurementMapDictionaryVersionCommand createVersionCommand,
            ICopyMeasurementMapDictionaryCommand copyCommand,
            ICreateMeasurementMapDictionaryItemCommand createItemCommand,
            IUpdateMeasurementMapDictionaryItemCommand updateItemCommand,
            IDeleteMeasurementMapDictionaryItemCommand deleteItemCommand,
            IGetAllRootItemsQuery getAllRootItemsQuery)
        {
            _getAllQuery = getAllQuery;
            _getByIdQuery = getByIdQuery;
            _createCommand = createCommand;
            _archiveCommand = archiveCommand;
            _createVersionCommand = createVersionCommand;
            _copyCommand = copyCommand;
            _createItemCommand = createItemCommand;
            _updateItemCommand = updateItemCommand;
            _deleteItemCommand = deleteItemCommand;
            _getAllRootItemsQuery = getAllRootItemsQuery;
            _dictionaries = new List<MeasurementMapDictionary>();
            _rootItems = new List<RootItem>();
        }

        // Загрузить список всех шапок
        public async Task LoadAllAsync()
        {
            var result = await _getAllQuery.ExecuteAsync();
            _dictionaries.Clear();
            _dictionaries.AddRange(result);

            if (_rootItems.Count == 0)
            {
                var rootItems = await _getAllRootItemsQuery.Execute();
                _rootItems.AddRange(rootItems);
            }

            DictionariesLoaded?.Invoke();
        }

        // Загрузить конкретный справочник со строками
        public async Task LoadCurrentAsync(int dictionaryId)
        {
            CurrentDictionary = await _getByIdQuery.ExecuteAsync(dictionaryId);
            CurrentDictionaryLoaded?.Invoke();
        }

        // Создать новый пустой справочник
        public async Task CreateAsync(MeasurementMapDictionary dictionary, IReadOnlyList<RootItem> rootItems, string createdBy)
        {
            var newDictionary = await _createCommand.ExecuteAsync(dictionary, rootItems, createdBy);
            dictionary.Id = newDictionary.Id;
            _dictionaries.Add(dictionary);
            DictionaryCreated?.Invoke(dictionary);
        }

        // Создать новый пустой справочник и скопировать в него items из другого справочника
        public async Task CopyAsync(int sourceMeasurementMapDictionaryId, MeasurementMapDictionary dictionary, IReadOnlyList<RootItem> rootItems, string createdBy)
        {
            var newId = await _copyCommand.ExecuteAsync(sourceMeasurementMapDictionaryId, dictionary, rootItems, createdBy);
            dictionary.Id = newId;
            _dictionaries.Add(dictionary);
            DictionaryCopied?.Invoke(dictionary);
        }

        // Архивировать справочник
        public async Task ArchiveAsync(int dictionaryId, string updatedBy)
        {
            await _archiveCommand.ExecuteAsync(dictionaryId, updatedBy);

            var idx = _dictionaries.FindIndex(d => d.Id == dictionaryId);
            if (idx >= 0)
                _dictionaries[idx].IsActive = false;

            DictionaryArchived?.Invoke(dictionaryId);
        }

        // Создать новую версию на основе существующего
        public async Task<int> CreateVersionAsync(int sourceDictionaryId, string createdBy)
        {
            var newId = await _createVersionCommand.ExecuteAsync(sourceDictionaryId, createdBy);

            // Обновляем список полностью — проще, чем patch в двух местах
            await LoadAllAsync();

            var newDict = _dictionaries.Find(d => d.Id == newId);
            DictionaryVersionCreated?.Invoke(newDict);

            return newId;
        }

        // Создать строку справочника
        public async Task CreateItemAsync(MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives, string createdBy)
        {
            var newId = await _createItemCommand.ExecuteAsync(item, repairMethodAlternatives, createdBy);
            item.Id = newId;

            // Перезагружаем CurrentDictionary чтобы получить актуальный список со строками
            if (CurrentDictionary != null)
                await LoadCurrentAsync(CurrentDictionary.Id);

            DictionaryItemCreated?.Invoke(item);
        }

        // Обновить строку справочника
        public async Task UpdateItemAsync(MeasurementMapDictionaryItem item,
            IReadOnlyList<RecommendedRepairMethodAlternative> repairMethodAlternatives, string updatedBy)
        {
            await _updateItemCommand.ExecuteAsync(item, repairMethodAlternatives, updatedBy);

            if (CurrentDictionary != null)
                await LoadCurrentAsync(CurrentDictionary.Id);

            DictionaryItemUpdated?.Invoke(item);
        }

        // Удалить строку справочника
        public async Task DeleteItemAsync(MeasurementMapDictionaryItem item, string updatedBy)
        {
            await _deleteItemCommand.ExecuteAsync(item, updatedBy);

            if (CurrentDictionary != null)
                await LoadCurrentAsync(CurrentDictionary.Id);

            DictionaryItemDeleted?.Invoke(item.Id);
        }
    }
}