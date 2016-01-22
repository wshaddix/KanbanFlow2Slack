using KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        internal async Task<Board> FetchBoardAsync()
        {
            var boardData = await GetStringAsync("board");
            return JsonConvert.DeserializeObject<Board>(boardData);
        }

        internal async Task<IEnumerable<User>> FetchUsersAsync()
        {
            var userData = await GetStringAsync("users");
            return JsonConvert.DeserializeObject<List<User>>(userData);
        }
    }
}