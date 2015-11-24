using Newtonsoft.Json;
using System.Collections.Generic;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class Task
    {
        internal string Color { get; set; }

        internal string ColumnId { get; set; }

        internal string ColumnName { get; set; }

        internal List<Date> Dates { get; set; }

        internal string Description { get; set; }

        [JsonProperty("_id")]
        internal string Id { get; set; }

        internal List<Label> Labels { get; set; }
        internal string Name { get; set; }
        internal List<SubTask> SubTasks { get; set; }
        internal string SwimlaneId { get; set; }
        internal string SwimlaneName { get; set; }
        internal long TotalSecondsEstimate { get; set; }
        internal long TotalSecondsSpent { get; set; }
    }
}