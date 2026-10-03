using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    // Загружает справочник со всеми его строками и альтернативами метода ремонта.
    // Используется при открытии формы редактирования строк конкретного справочника.
    public interface IGetMeasurementMapDictionaryByIdQuery
    {
        Task<MeasurementMapDictionary> ExecuteAsync(int dictionaryId);
    }
}