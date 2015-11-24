using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;
using KanbanFlow2Slack.Web.ApiClients.Slack;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Trace.TraceInformation($"Head() called from {HttpContext.Current.Request.UserHostAddress}");

            // nothing to do, we just need to return a 200 status code
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post()
        {
            Trace.TraceInformation($"Post() called from {HttpContext.Current.Request.UserHostAddress}");

            try
            {
                // extract the json data from the HTTP POST request
                var json = ExtractJsonFromPost();

                Trace.TraceInformation(json);

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
                Trace.TraceError(ex.ToString());
                return InternalServerError(ex);
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

                message = $"{userName} moved {taskLink} from {fromColumn.Name} to {toColumn.Name}";
            }
            else
            {
                message = $"{userName} just updated {taskLink}";
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