using Exceptionless;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using LogLevel = Exceptionless.Logging.LogLevel;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class WebhookEvent
    {
        // this constructor is called by JsonConvert.DeserializeObject<WebhookEvent>(json);
        public WebhookEvent()
        {
        }

        // this constructor is called by the WebHooksController.cs
        internal WebhookEvent(string json)
        {
            // convert the json to a WebhookEvent
            var instance = JsonConvert.DeserializeObject<WebhookEvent>(json, new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    var currentError = args.ErrorContext.Error.Message;

                    // we need to log that this happened but we can't do anything about it
                    ExceptionlessClient.Default.SubmitLog("WebhookEvent.ctor",
                        $"There was an error deserializing the webhook from KanbanFlow: {currentError}",
                        LogLevel.Error);

                    args.ErrorContext.Handled = true;
                }
            });

            if (null == instance)
            {
                // errors occurred during deserialization and we cannot continue
                throw new ApplicationException($"Failed to process webhook: {json}");
            }

            // populate this instances properties with the internal instance properties
            this.ChangedProperties = instance.ChangedProperties ?? new List<ChangedProperty>();
            this.EventType = instance.EventType;
            this.Task = instance.Task ?? new Task();
            this.Timestamp = instance.Timestamp;
            this.UserFullName = instance.UserFullName;
            this.UserId = instance.UserId;

            // set the UserFirstName property
            if (UserFullName.Contains(" "))
            {
                UserFirstName = UserFullName.Substring(0, UserFullName.IndexOf(" ", StringComparison.Ordinal));
            }
            else
            {
                UserFirstName = UserFullName;
            }

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
                var column = Globals.Board.Columns.FirstOrDefault(c => c.Id.Equals(Task.ColumnId));
                this.Task.ColumnName = (column == null) ? "unknown" : column.Name;

                // KanbanFlow does not provide the swimlane name as part of the webhook event so we
                // need to look it up from the cached board that we loaded when the application
                // started up.
                var swimlane = Globals.Board.SwimLanes.FirstOrDefault(sl => sl.Id.Equals(Task.SwimlaneId));
                this.Task.SwimlaneName = (swimlane == null) ? "unknown" : swimlane.Name;
            }
        }

        public List<ChangedProperty> ChangedProperties { get; set; }
        public string EventType { get; set; }
        public Task Task { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserFirstName { get; set; }
        public string UserFullName { get; set; }
        public string UserId { get; set; }
    }
}