using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace KanbanFlow2Slack.Web.ApiClients
{
    internal class SlackClient : HttpClient
    {
        internal SlackClient()
        {
            if (string.IsNullOrWhiteSpace(PostUrl))
            {
                throw new ArgumentNullException(nameof(PostUrl));
            }

            BaseAddress = new Uri(PostUrl);
        }

        internal static string PostUrl { get; set; }

        public void SendMessage(string message)
        {
            // convert the message into json format
            var payload = new
            {
                text = message
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var result = PostAsync(PostUrl, new StringContent(jsonPayload)).Result;
            result.EnsureSuccessStatusCode();
        }
    }
}