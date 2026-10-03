using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    // Создать экземпляр карты для строки ДВ на основе справочника.
    // Копирует позиции из MeasurementMapDictionary в MeasurementMapItem.
    // Пишет лог (Action = 1).
    // Возвращает созданный объект с заполненным Id и Items.
    public interface ICreateMeasurementMapCommand
    {
        Task<MeasurementMap> ExecuteAsync(int bomItemId, int codeLsf82, int rootItemId, string createdBy);
    }
}