using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class Column
    {
        [JsonProperty("uniqueId")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}