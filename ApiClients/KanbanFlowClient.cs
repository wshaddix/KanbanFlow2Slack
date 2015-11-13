using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace kanbanflow_slack
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

        public static string ApiKey { get; set; }

        internal Dictionary<string, string> FetchUsers()
        {
            var userData = GetStringAsync("users").Result;
            var users = JsonConvert.DeserializeObject<User[]>(userData);

            return users.ToDictionary(user => user.Id, user => user.FullName);
        }

        private class User
        {
            [JsonProperty("fullName")]
            internal string FullName { get; set; }

            [JsonProperty("_id")]
            internal string Id { get; set; }
        }
    }
}