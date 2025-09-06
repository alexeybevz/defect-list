using System;
using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllLogActionsQuery
    {
        List<LogRecord> BzrLogs(DateTime startDate, DateTime endDate);
        List<LogRecord> BzrLogs2(DateTime startDate, DateTime endDate);
        List<LogRecord> LogActionsByBomId(int bomId, DateTime startDate, DateTime endDate);
        List<LogRecord> BomItemLogByBomId(int bomId, DateTime startDate, DateTime endDate);
    }
}