using System;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class Date
    {
        public string DateType { get; set; }
        public DateTime DueTimestamp { get; set; }
        public DateTime DueTimestampLocal { get; set; }
        public string Status { get; set; }
        public string TargetColumnId { get; set; }
        public string TargetColumnName { get; set; }
    }
}