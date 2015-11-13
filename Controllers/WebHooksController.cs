using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;

namespace kanbanflow_slack.Controllers
{
    [Route("api/webhooks")]
    public class WebHooksController : Controller
    {
        // HEAD: api/webhooks
        [HttpHead]
        public void Head()
        {
            // nothing to do, we just need to return a 200 status code
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
            var sr = new StreamReader(HttpContext.Request.Body);
            var payload = sr.ReadToEnd();

            return JsonConvert.DeserializeObject(payload);
        }
    }
}