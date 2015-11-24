using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class SwimLane
    {
        [JsonProperty("uniqueId")]
        internal string Id { get; set; }

        internal string Name { get; set; }
    }
}