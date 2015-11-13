using KanbanFlow2Slack.Web.ApiClients;
using System.Configuration;
using System.Web.Http;

namespace KanbanFlow2Slack.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // setup configuration
            KanbanFlowClient.ApiKey = ConfigurationManager.AppSettings["kanbanflowApiKey"];
            SlackClient.PostUrl = ConfigurationManager.AppSettings["slackWebhookUrl"];
            Globals.ReportFirstNameOnly = bool.Parse(ConfigurationManager.AppSettings["reportFirstNameOnly"]);

            // fetch the list of users from kanbanflow
            Globals.Users = new KanbanFlowClient().FetchUsers();
        }
    }
}