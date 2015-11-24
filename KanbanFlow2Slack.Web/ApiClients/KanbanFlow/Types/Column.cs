using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class Column
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("uniqueId")]
        internal string Id { get; set; }
    }
}