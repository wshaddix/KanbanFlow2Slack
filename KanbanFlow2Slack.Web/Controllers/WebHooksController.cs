using KanbanFlow2Slack.Web.ApiClients;
using KanbanFlow2Slack.Web.Constants;
using KanbanFlow2Slack.Web.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
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

                Trace.TraceInformation(data);

                // extract the task meta-data
                var task = new Task(data);

                // generate the message to send to slack
                var message = GenerateMessage(task);

                // send the message to slack
                var slackClient = new SlackClient();
                slackClient.SendMessage(message);

                return "success";
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return ex.Message;
            }
        }

        private dynamic ExtractWebhookData()
        {
            var sr = new StreamReader(HttpContext.Current.Request.InputStream);
            var payload = sr.ReadToEnd();

            return JsonConvert.DeserializeObject(payload);
        }

        private string GenerateMessage(Task task)
        {
            // if the task was deleted then we don't want to generate a link to it
            return task.Action.Equals(TaskActions.Deleted) ? $"{task.User} just deleted {task.Name}" : $"{task.User} just {task.Action} <{string.Format(Globals.KanbanFlowUrlTemplate, task.Id)} | {task.Name}>";
        }
    }
}