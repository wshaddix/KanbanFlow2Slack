using Exceptionless;
using KanbanFlow2Slack.Web.ApiClients.KanbanFlow;
using KanbanFlow2Slack.Web.ApiClients.Slack;
using System.Configuration;
using System.Web.Http;

namespace KanbanFlow2Slack.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ExceptionlessClient.Default.RegisterWebApi(GlobalConfiguration.Configuration);
            ExceptionlessClient.Default.Configuration.ApiKey = ConfigurationManager.AppSettings["exceptionLessApiKey"];

            // setup configuration
            KanbanFlowClient.ApiKey = ConfigurationManager.AppSettings["kanbanflowApiKey"];
            SlackClient.PostUrl = ConfigurationManager.AppSettings["slackWebhookUrl"];
            Globals.ReportFirstNameOnly = bool.Parse(ConfigurationManager.AppSettings["reportFirstNameOnly"]);

            // fetch the list of boards from kanbanflow
            Globals.Board = new KanbanFlowClient().FetchBoard();
        }
    }
}