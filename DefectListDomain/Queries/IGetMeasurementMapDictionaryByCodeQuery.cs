using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetMeasurementMapDictionaryByCodeQuery
    {
        Task<MeasurementMapDictionary> ExecuteAsync(int codeLsf82, int rootItemId);
    }
}