using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateLogActionCommand : DbConnectionPmControlRepositoryBase, ICreateLogActionCommand
    {
        public List<LogActionType> LogActionTypes { get; }
        public Dictionary<ActionType, string> LogActionTypesDictionary => _logActionTypesDictionary;

        public CreateLogActionCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
            LogActionTypes = GetLogActionTypes();
        }

        public void Execute(LogActionType logActionType, int bomId, string context, string text, string userText, string login)
        {
            if (LogActionTypes.Count(x => x.LogActionTypeName == logActionType.LogActionTypeName) == 0)
            {
                throw new ArgumentException("Указан некорретный тип логгируемого события");
            }

            using (var db = CreateOpenConnection())
            {
                var parms= new
                {
                    logActionType.LogActionTypeId,
                    BomId = bomId,
                    LogActionContext = context,
                    LogActionText = text,
                    UserActionText = userText,
                    CreatedBy = login,
                };

                db.Execute(@"INSERT INTO LogAction (LogActionTypeId, BomId, LogActionContext, LogActionText, UserActionText, CreatedBy)
                                VALUES (@LogActionTypeId, @BomId, @LogActionContext, @LogActionText, @UserActionText, @CreatedBy)", parms);
            }
        }

        private List<LogActionType> GetLogActionTypes()
        {
            using (var db = CreateOpenConnection())
            {
                return db.Query<LogActionType>("SELECT * FROM LogActionType").ToList();
            }
        }

        private readonly Dictionary<ActionType, string> _logActionTypesDictionary = new Dictionary<ActionType, string>()
        {
            { ActionType.Add, "BomItemAdd" },
            { ActionType.Replace, "BomItemReplace" },
            { ActionType.ReplaceName, "BomItemReplaceName" },
            { ActionType.Delete, "BomItemDelete" },
            { ActionType.BomHeaderChanged, "BomHeaderChanged" },
        };
    }
}
