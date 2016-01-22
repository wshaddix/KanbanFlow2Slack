using Exceptionless;
using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;
using KanbanFlow2Slack.Web.ApiClients.Slack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace KanbanFlow2Slack.Web.Controllers
{
    [Route("api/webhooks")]
    public class WebHooksController : ApiController
    {
        // HEAD: api/webhooks
        [HttpHead]
        public IHttpActionResult Head()
        {
            ExceptionlessClient.Default.SubmitLog(typeof(WebHooksController).FullName, "HEAD called",
                Exceptionless.Logging.LogLevel.Info);
            ExceptionlessClient.Default.SubmitFeatureUsage("KanbanFlow2Slack.WebHooksController.Head()");

            // nothing to do, we just need to return a 200 status code
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post()
        {
            try
            {
                ExceptionlessClient.Default.SubmitFeatureUsage("KanbanFlow2Slack.WebHooksController.Post()");

                // extract the json data from the HTTP POST request
                var json = ExtractJsonFromPost();

                ExceptionlessClient.Default.SubmitLog(typeof(WebHooksController).FullName, json, Exceptionless.Logging.LogLevel.Info);

                // create a new webhook event from the json
                var webhookEvent = new WebhookEvent(json);

                // generate the message to send to slack
                var message = GenerateMessage(webhookEvent);

                // send the message to slack
                var slackClient = new SlackClient();
                slackClient.SendMessage(message);

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return Ok();
            }
        }

        private string ExtractJsonFromPost()
        {
            var sr = new StreamReader(HttpContext.Current.Request.InputStream);
            return sr.ReadToEnd();
        }

        private string GenerateMessage(WebhookEvent webhookEvent)
        {
            // the messaging varies based on the tasks action
            string message;

            switch (webhookEvent.EventType)
            {
                case EventTypes.Created:
                    message = GenerateTaskCreatedMessage(webhookEvent);
                    break;

                case EventTypes.Changed:
                    message = GenerateTaskUpdatedMessage(webhookEvent);
                    break;

                case EventTypes.Deleted:
                    message = GenerateTaskDeletedMessage(webhookEvent);
                    break;

                default:
                    message = "We have no idea what just happened!";
                    break;
            }

            return message;
        }

        private string GenerateTaskCreatedMessage(WebhookEvent webhookEvent)
        {
            // figure out which name to use
            var userName = GetUsername(webhookEvent);

            // generate the task link
            var taskLink = GetTaskLink(webhookEvent);

            // we want to generate a url link in slack back to the kanbanflow task so the slack user
            // can click the link and view the details
            return $"{userName} just created {taskLink}";
        }

        private string GenerateTaskDeletedMessage(WebhookEvent webhookEvent)
        {
            // figure out which name to use
            var userName = GetUsername(webhookEvent);

            // we don't want to generate a link to the task since it no longer exists
            return $"{userName} just deleted {webhookEvent.Task.Name}";
        }

        private string GenerateTaskUpdatedMessage(WebhookEvent webhookEvent)
        {
            string message;

            // figure out which name to use
            var userName = GetUsername(webhookEvent);

            // generate the task link
            var taskLink = GetTaskLink(webhookEvent);

            // if the user updated the column, we want to know that in more context in slack
            // otherwise we just tell the user what changed
            if (webhookEvent.ChangedProperties.Any(p => p.Property.ToLower().Equals("columnid")))
            {
                // the user moved the task between columns so let's make an informative message
                var columnChangeProperty = webhookEvent.ChangedProperties.First(p => p.Property.ToLower().Equals("columnid"));
                var fromColumn = Globals.Board.Columns.First(c => c.Id.Equals(columnChangeProperty.OldValue));
                var toColumn = Globals.Board.Columns.First(c => c.Id.Equals(columnChangeProperty.NewValue));

                message = $"{userName} moved {taskLink} from *{fromColumn.Name}* to *{toColumn.Name}*";
            }
            // if the user updated the totalSecondsSpent we want to display it in a more readable format
            else if (webhookEvent.ChangedProperties.Any(p => p.Property.ToLower().Equals("totalsecondsspent")))
            {
                var timeChangeProperty = webhookEvent.ChangedProperties.First(p => p.Property.ToLower().Equals("totalsecondsspent"));
                var oldTime = double.Parse(timeChangeProperty.OldValue);
                var newTime = double.Parse(timeChangeProperty.NewValue);

                var oldTotalTime = TimeSpan.FromSeconds(oldTime);
                var newTotalTime = TimeSpan.FromSeconds(newTime);

                message = $"{userName} updated time spent on {taskLink} from *{oldTotalTime.Hours}h{oldTotalTime.Minutes}m{oldTotalTime.Seconds}s* to *{newTotalTime.Hours}h{newTotalTime.Minutes}m{newTotalTime.Seconds}s*";
            }
            // if the responsible user was updated we want to display the user's name instead of the userId
            else if (webhookEvent.ChangedProperties.Any(p => p.Property.ToLower().Equals("responsibleuserid")))
            {
                var responsibleUserProperty = webhookEvent.ChangedProperties.First(p => p.Property.ToLower().Equals("responsibleuserid"));
                var userId = responsibleUserProperty.NewValue;
                var responsibleUser = Globals.Users.FirstOrDefault(u => u.Id == userId);

                message = $"{userName} assigned {taskLink} to {responsibleUser?.FullName}";
            }
            else
            {
                // the user changed the value of one or more columns
                var sb = new StringBuilder();
                sb.AppendLine($"{userName} just updated {taskLink}");

                foreach (var property in webhookEvent.ChangedProperties)
                {
                    sb.AppendLine($"*{property.Property}* was updated to: {property.NewValue}");
                }

                message = sb.ToString();
            }

            return message;
        }

        private string GetTaskLink(WebhookEvent webhookEvent)
        {
            return $"<{string.Format(Globals.KanbanFlowUrlTemplate, webhookEvent.Task.Id)} | {webhookEvent.Task.Name}>";
        }

        private string GetUsername(WebhookEvent webhookEvent)
        {
            return Globals.ReportFirstNameOnly ? webhookEvent.UserFirstName : webhookEvent.UserFullName;
        }
    }
}