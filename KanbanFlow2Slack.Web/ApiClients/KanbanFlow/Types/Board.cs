using Newtonsoft.Json;
using System.Collections.Generic;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class Board
    {
        public List<Column> Columns { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public List<SwimLane> SwimLanes { get; set; }
    }
}