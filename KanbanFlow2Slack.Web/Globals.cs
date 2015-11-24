using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;
using System.Collections.Generic;

namespace KanbanFlow2Slack.Web
{
    internal static class Globals
    {
        internal static string KanbanFlowUrlTemplate = @"https://kanbanflow.com/t/{0}";
        internal static Board Board { get; set; }
        internal static bool ReportFirstNameOnly { get; set; }
        internal static List<User> Users { get; set; }
    }
}