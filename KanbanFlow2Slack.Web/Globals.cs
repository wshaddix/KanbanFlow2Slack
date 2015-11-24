using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;

namespace KanbanFlow2Slack.Web
{
    internal static class Globals
    {
        internal static string KanbanFlowUrlTemplate = @"https://kanbanflow.com/t/{0}";
        internal static Board Board { get; set; }
        internal static bool ReportFirstNameOnly { get; set; }
    }
}