using Newtonsoft.Json;
using System.Collections.Generic;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class Board
    {
        internal List<Column> Columns { get; set; }

        [JsonProperty("_id")]
        internal string Id { get; set; }

        internal string Name { get; set; }

        internal List<SwimLane> SwimLanes { get; set; }
    }
}