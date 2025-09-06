using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllDecisionsQuery : IGetAllDecisionsQuery
    {
        public async Task<IEnumerable<string>> Execute()
        {
            return await Task.Run(() => new List<string>()
            {
                "заменить",
                "использовать",
                "ремонт",
                "ремонтное воздействие не требуется",
                "скомплектовать"
            });
        }
    }
}