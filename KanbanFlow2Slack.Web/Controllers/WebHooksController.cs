using KanbanFlow2Slack.Web.ApiClients;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace KanbanFlow2Slack.Web.Controllers
{
    [Route("api/webhooks")]
    public class WebHooksController : ApiController
    {
        // HEAD: api/webhooks
        [HttpHead]
        public OkResult Head()
        {
            // nothing to do, we just need to return a 200 status code
            return Ok();
        }

        [HttpPost]
        public string Post()
        {
            try
            {
                // convert the webhook data into json
                var data = ExtractWebhookData();

                // extract the task meta-data
                var task = ExtractTask(data);

                // generate the message to send to slack
                var message = GenerateMessage(task);

                // send the message to slack
                var slackClient = new SlackClient();
                slackClient.SendMessage(message);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string DetermineAction(dynamic data)
        {
            var eventType = (string)data.eventType.Value;
            string action;

            switch (eventType)
            {
                case "taskCreated":
                    {
                        action = "created";
                        break;
                    }
                case "taskUpdated":
                    {
                        action = "updated";
                        break;
                    }
                case "taskDeleted":
                    {
                        action = "deleted";
                        break;
                    }
                default:
                    {
                        action = "unknown";
                        break;
                    }
            }

            return action;
        }

        private string DetermineUser(dynamic data)
        {
            var userId = data.userId.Value;
            string user;
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
                user = user?.Substring(0, user.IndexOf(" ", StringComparison.Ordinal) + 1);
            }

            return string.IsNullOrWhiteSpace(user) ? "unknown" : user;
        }

        private Task ExtractTask(dynamic data)
        {
            // determine what action was taken on the kanbanflow task
            var action = DetermineAction(data);

            // determine which user took the action
            var user = DetermineUser(data);

            return new Task
            {
                Id = data.task._id,
                Name = data.task.name,
                User = user,
                Action = action
            };
        }

        private dynamic ExtractWebhookData()
        {
            var sr = new StreamReader(HttpContext.Current.Request.InputStream);
            var payload = sr.ReadToEnd();

            return JsonConvert.DeserializeObject(payload);
        }

        private string GenerateMessage(Task task)
        {
            return $"{task.User} just {task.Action} <{string.Format(Globals.KanbanFlowUrlTemplate, task.Id)} | {task.Name}>";
        }

        private class Task
        {
            internal string Action { get; set; }
            internal string Id { get; set; }
            internal string Name { get; set; }
            internal string User { get; set; }
        }
    }
}