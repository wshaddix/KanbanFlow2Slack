using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class WebhookEvent
    {
        internal WebhookEvent(string json)
        {
            // instantiate any reference types to avoid possible null ref exceptions
            ChangedProperties = new List<ChangedProperty>();
            Task = new Task();

            // convert the json to a WebhookEvent
            var instance = JsonConvert.DeserializeObject<WebhookEvent>(json);

            // populate this instances properties with the internal instance properties
            this.ChangedProperties = instance.ChangedProperties;
            this.EventType = instance.EventType;
            this.Task = instance.Task;
            this.Timestamp = instance.Timestamp;
            this.UserFullName = instance.UserFullName;
            this.UserId = instance.UserId;

            // set the UserFirstName property
            UserFirstName = UserFullName.Substring(0, UserFullName.IndexOf(" ", StringComparison.Ordinal));

            // if the EventType is taskDeleted, then KanbanFlow places the taskId and taskName in a
            // different part of the json payload than if the EventType is taskChanged or taskCreated
            if (EventType.Equals(EventTypes.Deleted))
            {
                // deserialize the json string as a dynamic object
                dynamic data = JsonConvert.DeserializeObject(json);
                Task.Id = data.taskId.Value;
                Task.Name = data.taskName.Value;
            }
            else
            {
                // KanbanFlow does not provide the column name as part of the webhook event so we
                // need to look it up from the cached board that we loaded when the application
                // started up.
                this.Task.ColumnName = Globals.Board.Columns.First(c => c.Id.Equals(Task.ColumnId)).Name;

                // KanbanFlow does not provide the swimlane name as part of the webhook event so we
                // need to look it up from the cached board that we loaded when the application
                // started up.
                this.Task.SwimlaneName = Globals.Board.SwimLanes.First(sl => sl.Id.Equals(Task.SwimlaneId)).Name;
            }
        }

        internal List<ChangedProperty> ChangedProperties { get; set; }
        internal string EventType { get; set; }
        internal Task Task { get; set; }
        internal DateTime Timestamp { get; set; }
        internal string UserFirstName { get; set; }
        internal string UserFullName { get; set; }
        internal string UserId { get; set; }
    }
}