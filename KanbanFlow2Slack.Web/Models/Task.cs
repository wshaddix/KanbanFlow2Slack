using KanbanFlow2Slack.Web.ApiClients;
using KanbanFlow2Slack.Web.Constants;
using System;

namespace KanbanFlow2Slack.Web.Models
{
    internal class Task
    {
        internal Task(dynamic data)
        {
            // extract the task meta-data
            ExtractTask(data);
        }

        internal string Action { get; set; }
        internal string Id { get; set; }
        internal string Name { get; set; }
        internal string User { get; set; }

        private void DetermineAction(dynamic data)
        {
            var eventType = (string)data.eventType.Value;

            switch (eventType)
            {
                case "taskCreated":
                    {
                        Action = TaskActions.Created;
                        break;
                    }
                case "taskChanged":
                    {
                        Action = TaskActions.Updated;
                        break;
                    }
                case "taskDeleted":
                    {
                        Action = TaskActions.Deleted;
                        break;
                    }
                default:
                    {
                        Action = TaskActions.Unknown;
                        break;
                    }
            }
        }

        private void DetermineNameAndId(dynamic data)
        {
            string id;
            string name;

            // the task id and task name are in different places depending on the action :(
            if (Action.Equals(TaskActions.Created) || Action.Equals(TaskActions.Deleted))
            {
                id = data.task._id;
                name = data.task.name;
            }
            else
            {
                id = data.taskId;
                name = data.taskName;
            }

            Id = id;
            Name = name;
        }

        private void DetermineUser(dynamic data)
        {
            var userId = data.userId.Value;
            var user = string.Empty;
            Globals.Users.TryGetValue(userId, out user);

            // if the user was not found, it might be because the user was added to kanbanflow
            // *after* our application startup where we cache the users. Let's reload the user cache
            // and re-check
            if (string.IsNullOrWhiteSpace(user))
            {
                Globals.Users = new KanbanFlowClient().FetchUsers();
                Globals.Users.TryGetValue(userId, out user);
            }

            // if we are only using first names then ignore anything after the first space character
            if (Globals.ReportFirstNameOnly)
            {
                user = user?.Substring(0, user.IndexOf(" ", StringComparison.Ordinal));
            }

            User = string.IsNullOrWhiteSpace(user) ? "unknown" : user;
        }

        private void ExtractTask(dynamic data)
        {
            // determine what action was taken on the kanbanflow task
            DetermineAction(data);

            // determine which user took the action
            DetermineUser(data);

            // determine the name and id of the task
            DetermineNameAndId(data);
        }
    }
}