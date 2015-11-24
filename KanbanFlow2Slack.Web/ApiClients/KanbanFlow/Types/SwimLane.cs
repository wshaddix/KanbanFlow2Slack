using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class SwimLane
    {
        [JsonProperty("uniqueId")]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}