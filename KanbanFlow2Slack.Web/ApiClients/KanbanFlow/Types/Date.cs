using System;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class Date
    {
        internal string DateType { get; set; }
        internal DateTime DueTimestamp { get; set; }
        internal DateTime DueTimestampLocal { get; set; }
        internal string Status { get; set; }
        internal string TargetColumnId { get; set; }
        internal string TargetColumnName { get; set; }
    }
}