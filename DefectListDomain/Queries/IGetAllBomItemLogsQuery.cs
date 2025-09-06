using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllBomItemLogsQuery
    {
        Task<IEnumerable<BomItemLog>> ExecuteByBomId(int bomId);
        Task<IEnumerable<BomItemLog>> ExecuteByBomItemId(int bomItemId);
        Task<IEnumerable<FinalDecisionChanging>> FinalDecisionChangings(DateTime startDate, DateTime endDate);
    }
}