using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class User
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}