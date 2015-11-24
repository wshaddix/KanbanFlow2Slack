using Newtonsoft.Json;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class User
    {
        [JsonProperty("fullName")]
        internal string FullName { get; set; }

        [JsonProperty("_id")]
        internal string Id { get; set; }
    }
}