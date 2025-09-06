using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateStateBomHeaderCommand : DbConnectionPmControlRepositoryBase, IUpdateStateBomHeaderCommand
    {
        private readonly ICreateLogActionCommand _createLogActionCommand;
        public UpdateStateBomHeaderCommand(IDbConnectionFactory dbConnectionFactory, ICreateLogActionCommand createLogActionCommand) : base(dbConnectionFactory)
        {
            _createLogActionCommand = createLogActionCommand;
        }

        public async Task Execute(BomHeader bomHeader)
        {
            var query = "UPDATE BomHeader SET State = @State, UpdatedBy = @UpdatedBy, RecordDate = @RecordDate WHERE BomId = @BomId";
            using (var db = await CreateOpenConnectionAsync())
            {
                await db.ExecuteAsync(query, new { bomHeader.State, bomHeader.UpdatedBy, bomHeader.RecordDate, bomHeader.BomId });

                switch (bomHeader.State)
                {
                    case BomHeaderState.WorkInProgress:
                        WriteLog(bomHeader, $"ID ведомости {{{bomHeader.BomId}}}. Ведомость находится в работе",
                            $"ДВ {{{bomHeader.Orders}}} в работе");
                        break;
                    case BomHeaderState.Approved:
                        WriteLog(bomHeader, $"ID ведомости {{{bomHeader.BomId}}}. Ведомость утверждена",
                            $"ДВ {{{bomHeader.Orders}}} утверждена");
                        break;
                    case BomHeaderState.Closed:
                        WriteLog(bomHeader, $"ID ведомости {{{bomHeader.BomId}}}. Ведомость закрыта",
                            $"ДВ {{{bomHeader.Orders}}} закрыта");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void WriteLog(BomHeader bomHeader, string logActionText, string userActionText)
        {
            var logActionType = _createLogActionCommand.LogActionTypes.FirstOrDefault(x => x.LogActionTypeName == _createLogActionCommand.LogActionTypesDictionary[ActionType.BomHeaderChanged]);

            _createLogActionCommand.Execute(
                logActionType,
                bomHeader.BomId,
                "BomHeader",
                logActionText,
                userActionText,
                bomHeader.UpdatedBy);
        }
    }
}