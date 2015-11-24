using Newtonsoft.Json;
using System.Collections.Generic;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class Task
    {
        public string Color { get; set; }
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
        public List<Date> Dates { get; set; }
        public string Description { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        public List<Label> Labels { get; set; }
        public string Name { get; set; }
        public List<SubTask> SubTasks { get; set; }
        public string SwimlaneId { get; set; }
        public string SwimlaneName { get; set; }
        public long TotalSecondsEstimate { get; set; }
        public long TotalSecondsSpent { get; set; }
    }
}