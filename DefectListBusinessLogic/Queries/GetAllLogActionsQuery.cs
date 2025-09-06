using Dapper;
using System;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Queries;
using System.Collections.Generic;
using System.Linq;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllLogActionsQuery : DbConnectionPmControlRepositoryBase, IGetAllLogActionsQuery
    {
        public GetAllLogActionsQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public List<LogRecord> BzrLogs2(DateTime startDate, DateTime endDate)
        {
            var query = GetQueryFromLogAction() + "WHERE a.LogActionText like '%БЗР%' AND a.CreateDate BETWEEN @startDate and @endDate";
            return DbConnection.Query<LogRecord>(query, new { startDate, endDate }).ToList();
        }

        public List<LogRecord> LogActionsByBomId(int bomId, DateTime startDate, DateTime endDate)
        {
            var query = GetQueryFromLogAction() + "WHERE a.BomId = @BomId AND a.CreateDate BETWEEN @startDate and @endDate";
            return DbConnection.Query<LogRecord>(query, new { BomId = bomId, startDate, endDate }).ToList();
        }

        public List<LogRecord> BomItemLogByBomId(int bomId, DateTime startDate, DateTime endDate)
        {
            var query = GetQueryFromBomItemLog("WHERE b.CreateDate BETWEEN @startDate AND @endDate AND b.BomId = @BomId AND b.Action > 0");
            return DbConnection.Query<LogRecord>(query, new { BomId = bomId, startDate, endDate }).ToList();
        }

        public List<LogRecord> BzrLogs(DateTime startDate, DateTime endDate)
        {
            var query = GetQueryFromBomItemLog("WHERE b.CreateDate BETWEEN @startDate AND @endDate AND b.Detal LIKE '%БЗР%' AND b.Action > 0");
            return DbConnection.Query<LogRecord>(query, new { startDate, endDate }).ToList();
        }


        private string GetQueryFromLogAction()
        {
            return @"SELECT
                          a.UserActionText AS ActionText
	                    , a.CreateDate
	                    , u.ActiveDirectoryCN AS CreatedBy
                    FROM LogAction a
                    LEFT JOIN Users u ON u.Login = a.CreatedBy ";
        }

        private string GetQueryFromBomItemLog(string whereCondition)
        {
            return $@"with logs AS (
            select
                  bh.Orders
                , b.Detal
                , b.DetalIma
                , b.BomItemDocId
                , MAX(b.Id) AS MaxId
                , MIN(b.Id) AS MinId
            from BomItemLog b
            left join BomHeader bh ON bh.BomId = b.BomId  
            {whereCondition}
            group by
                  bh.Orders
                , b.Detal
                , b.DetalIma
                , BomItemDocId
            ), prevLogs as (
	            select dt.*, (select top 1 b.id from BomItemLog b where b.BomItemDocId = dt.BomItemDocId AND b.Id < dt.MinId order by b.id desc) as prevId
	            from logs dt
            )
            select
	              dt.Orders
	            , dt.Detal
	            , dt.DetalIma

	            , bprev.Defect
	            , bprev.Decision
	            , bprev.FinalDecision
	            , bprev.SerialNumber

	            , b.Defect AS DefectNew
	            , b.Decision AS DecisionNew
	            , b.FinalDecision AS FinalDecisionNew
	            , b.SerialNumber AS SerialNumberNew
	            
                , CASE WHEN b.Action = 1 THEN 'Изменение' WHEN b.Action = 2 THEN 'Удаление' ELSE '' END ActionText
	            , b.CreateDate
	            , u.ActiveDirectoryCN AS CreatedBy
            from logs as dt
            inner join BomItemLog b ON b.Id = dt.MaxId
            left join prevLogs p ON p.BomItemDocId = dt.BomItemDocId
            left join BomItemLog bprev ON bprev.Id = p.prevId
            left join Users u ON u.Login = b.CreatedBy
            order by dt.Orders, dt.Detal, b.CreateDate";
        }
    }
}