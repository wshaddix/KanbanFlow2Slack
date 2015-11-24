using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class User
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }
    }
}