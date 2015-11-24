using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow
{
    internal class KanbanFlowClient : HttpClient
    {
        internal KanbanFlowClient()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new ArgumentNullException(nameof(ApiKey));
            }
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("apiToken:" + ApiKey));
            BaseAddress = new Uri("https://kanbanflow.com/api/v1/");
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
        }

        internal static string ApiKey { get; set; }

        internal Board FetchBoard()
        {
            var boardData = GetStringAsync("board").Result;
            return JsonConvert.DeserializeObject<Board>(boardData);
        }
    }
}