using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListDomain.Services.NotificationDispatchesService;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public class UserBomHeaderSubscriptionDigestBuilder : IUserBomHeaderSubscriptionDigestBuilder
    {
        private readonly IGetAllLogActionsQuery _getAllLogActionsQuery;
        private readonly IGetAllBomHeaderSubscribersQuery _getAllBomHeaderSubscribersQuery;

        public UserBomHeaderSubscriptionDigestBuilder(
            IGetAllLogActionsQuery getAllLogActionsQuery,
            IGetAllBomHeaderSubscribersQuery getAllBomHeaderSubscribersQuery)
        {
            _getAllLogActionsQuery = getAllLogActionsQuery;
            _getAllBomHeaderSubscribersQuery = getAllBomHeaderSubscribersQuery;
        }

        public async Task<string> ExecuteAsync(int userId, DateTime digestDate)
        {
            var usersBomHeadersSubscription = (await _getAllBomHeaderSubscribersQuery.Execute()).ToList()
                .GroupBy(x => x.UserId)
                .ToDictionary(x => x.Key,
                    x => x.Select(s => s.BomId).ToList());

            List<int> userBomHeadersSubscription;
            if (!usersBomHeadersSubscription.TryGetValue(userId, out userBomHeadersSubscription))
                return string.Empty;

            var data = new List<LogRecord>();
            var data2 = new List<LogRecord>();

            foreach (var bomId in userBomHeadersSubscription)
            {
                data.AddRange(_getAllLogActionsQuery.BomItemLogByBomId(bomId, digestDate.AddDays(-1), digestDate));
                data2.AddRange(_getAllLogActionsQuery.LogActionsByBomId(bomId, digestDate.AddDays(-1), digestDate));
            }

            if (!data.Any() && !data2.Any())
                return string.Empty;

            var sb = new StringBuilder();
            CreateTableBomItemAttributesChanges(sb, data);
            CreateTableBomViewChages(sb, data2);

            return sb.ToString();
        }

        private static void CreateTableBomItemAttributesChanges(StringBuilder sb, List<LogRecord> data)
        {
            if (!data.Any())
                return;

            sb.AppendLine("<p>Изменения атрибутов:</p>");

            sb.AppendLine("<table border=\"1\" cellspacing=\"0\" cellpadding=\"2\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>Заказ</td>");
            sb.AppendLine("<td>Обозначение ДСЕ</td>");
            sb.AppendLine("<td>Дефект Old</td>");
            sb.AppendLine("<td>Дефект New</td>");
            sb.AppendLine("<td>Решение Old</td>");
            sb.AppendLine("<td>Решение New</td>");
            sb.AppendLine("<td>Окончательное решение Old</td>");
            sb.AppendLine("<td>Окончательное решение New</td>");
            sb.AppendLine("<td>Серийный номер Old</td>");
            sb.AppendLine("<td>Серийный номер New</td>");
            sb.AppendLine("<td>Тип действия</td>");
            sb.AppendLine("<td>Дата изменения</td>");
            sb.AppendLine("<td>Пользователь</td>");
            sb.AppendLine("</tr>");

            data.ForEach(d =>
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{d.Orders}</td>");
                sb.AppendLine($"<td>{d.Detal}</td>");
                sb.AppendLine($"<td>{d.Defect}</td>");
                sb.AppendLine($"<td>{d.DefectNew}</td>");
                sb.AppendLine($"<td>{d.Decision}</td>");
                sb.AppendLine($"<td>{d.DecisionNew}</td>");
                sb.AppendLine($"<td>{d.FinalDecision}</td>");
                sb.AppendLine($"<td>{d.FinalDecisionNew}</td>");
                sb.AppendLine($"<td>{d.SerialNumber}</td>");
                sb.AppendLine($"<td>{d.SerialNumberNew}</td>");
                sb.AppendLine($"<td>{d.ActionText}</td>");
                sb.AppendLine($"<td>{d.CreateDate}</td>");
                sb.AppendLine($"<td>{d.CreatedBy}</td>");
                sb.AppendLine("</tr>");
            });

            sb.AppendLine("</table>");
        }

        private static void CreateTableBomViewChages(StringBuilder sb, List<LogRecord> data2)
        {
            if (!data2.Any())
                return;

            sb.AppendLine("<p>Корректировка состава:</p>");

            sb.AppendLine("<table border=\"1\" cellspacing=\"0\" cellpadding=\"2\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>Событие</td>");
            sb.AppendLine("<td>Дата</td>");
            sb.AppendLine("<td>Пользователь</td>");
            sb.AppendLine("</tr>");

            data2.ForEach(d =>
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{d.ActionText}</td>");
                sb.AppendLine($"<td>{d.CreateDate}</td>");
                sb.AppendLine($"<td>{d.CreatedBy}</td>");
                sb.AppendLine("</tr>");
            });

            sb.AppendLine("</table>");
        }
    }
}