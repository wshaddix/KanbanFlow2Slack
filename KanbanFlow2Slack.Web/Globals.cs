using System.Collections.Generic;

namespace KanbanFlow2Slack.Web
{
    internal static class Globals
    {
        public static string KanbanFlowUrlTemplate = @"https://kanbanflow.com/t/{0}";
        public static bool ReportFirstNameOnly { get; set; }
        internal static Dictionary<string, string> Users { get; set; }
    }
}