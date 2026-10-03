using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefectListDomain.ExternalData
{
    public interface IGetAllDistinctShopEntriesQuery
    {
        /// <summary>
        /// Возвращает список уникальных цехозаходов (Value: цех-уч1; цех-уч2;) в технологическом процессе на ДСЕ (Id)
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, string>> Execute();
    }
}