using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Сохранить фактические значения сразу по всем позициям карты за один вызов.
    // Используется при нажатии "Сохранить" на форме.
    // Пишет лог по каждой изменённой позиции (Action = 2).
    public interface ISaveMeasurementMapItemsCommand
    {
        Task ExecuteAsync(int measurementMapId, IReadOnlyCollection<MeasurementMapItem> items, string updatedBy);
    }
}