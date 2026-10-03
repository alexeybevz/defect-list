using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    public class MeasurementMapStore
    {
        private readonly IGetMeasurementMapByBomItemIdQuery _getMapQuery;
        private readonly ICreateMeasurementMapCommand _createMapCommand;
        private readonly ISaveMeasurementMapItemsCommand _saveMeasurementMapItemsCommand;

        private MeasurementMap _measurementMap;

        // Текущий загруженный экземпляр карты. Null = карта ещё не создана.
        public MeasurementMap MeasurementMap => _measurementMap;
        public bool HasMeasurementMap => _measurementMap != null;

        // --- События ---

        // Карта загружена (или установлена в null — карты нет)
        public event Action MeasurementMapLoaded;

        // Карта создана впервые для этой строки ДВ
        public event Action<MeasurementMap> MeasurementMapCreated;

        // Фактические значения сохранены
        public event Action MeasurementMapItemsSaved;

        public MeasurementMapStore(
            IGetMeasurementMapByBomItemIdQuery getMapQuery,
            ICreateMeasurementMapCommand createMapCommand,
            ISaveMeasurementMapItemsCommand saveMeasurementMapItemsCommand)
        {
            _getMapQuery = getMapQuery;
            _createMapCommand = createMapCommand;
            _saveMeasurementMapItemsCommand = saveMeasurementMapItemsCommand;
        }

        // Загрузить карту для строки ДВ.
        // Если карты нет — _measurementMap = null, событие всё равно стреляет.
        public async Task LoadAsync(int bomItemId)
        {
            _measurementMap = await _getMapQuery.ExecuteAsync(bomItemId);
            MeasurementMapLoaded?.Invoke();
        }

        // Создать карту на основе справочника для данного Code_LSF82.
        // Сначала находим активный справочник, потом создаём экземпляр.
        public async Task CreateAsync(int bomItemId, int codeLsf82, int rootItemId, string createdBy)
        {
            var map = await _createMapCommand.ExecuteAsync(bomItemId, codeLsf82, rootItemId, createdBy);
            _measurementMap = map;
            MeasurementMapCreated?.Invoke(map);
        }

        // Сохранить фактические значения всех позиций.
        public async Task SaveItemsAsync(IReadOnlyCollection<MeasurementMapItem> items, string updatedBy)
        {
            await _saveMeasurementMapItemsCommand.ExecuteAsync(_measurementMap.Id, items, updatedBy);
            MeasurementMapItemsSaved?.Invoke();
        }
    }
}