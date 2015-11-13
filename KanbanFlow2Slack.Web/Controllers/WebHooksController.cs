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
                var data = ExtractWebhookData();
                var userId = data.userId.Value;
                var eventType = data.eventType.Value;
                var taskName = data.task.name;
                var taskId = data.task._id;

                var action = eventType == "taskCreated" ? "created" : "updated";
                var username = Globals.Users[userId];

                var message =
                    $"{username} just {action} <{string.Format(Globals.KanbanFlowUrlTemplate, taskId.Value)} | {taskName.Value}>";

                var slackClient = new SlackClient();
                slackClient.SendMessage(message);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private dynamic ExtractWebhookData()
        {
            var sr = new StreamReader(HttpContext.Current.Request.InputStream);
            var payload = sr.ReadToEnd();

            return JsonConvert.DeserializeObject(payload);
        }
    }
}