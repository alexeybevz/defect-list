using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetMeasurementsMapTypeFormsQuery
    {
        Task<IReadOnlyList<MeasurementsMapTypeForm>> ExecuteAsync();
    }
}