using System.Collections.Generic;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateLogActionCommand
    {
        List<LogActionType> LogActionTypes { get; }
        Dictionary<ActionType, string> LogActionTypesDictionary { get; }
        void Execute(LogActionType logActionType, int bomId, string context, string text, string userText, string login);
    }
}