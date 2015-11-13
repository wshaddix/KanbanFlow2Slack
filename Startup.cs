using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace kanbanflow_slack
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();

            // setup configuration
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            var config = builder.Build();

            KanbanFlowClient.ApiKey = config.Get<string>("kanbanflowApiKey");
            SlackClient.PostUrl = config.Get<string>("slackWebhookUrl");

            // fetch the list of users from kanbanflow
            Globals.Users = new KanbanFlowClient().FetchUsers();
        }

        // This method gets called by a runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port
            // Web API 2 controllers. You will also need to add the
            // Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of
            // project.json. services.AddWebApiConventions();
        }
    }
}