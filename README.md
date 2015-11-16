# KanbanFlow2Slack
Integrates KanbanFlow with Slack.

[KanbanFlow](https://kanbanflow.com) has integration with [Slack](https://slack.com/) via [Zapier](https://zapier.com/developer/invite/29685/72eafdb0a0aec39db628eeca98d336c6) but at the time this project was created it didn't give you access to the user's name that created/updated/deleted the tasks. Also, it took multiple zaps to get notified when create/update actions happened.

Not wanting to create a Zapier account and use up more than one of the five free zaps I created this project to run in [Azure](https://azure.microsoft.com/en-us/) and receive webhooks from KanbanFlow and then notify Slack.

How it works
============
1. When the application starts up, it queries KanbanFlow's API for a list of all users associated with your account.
2. When KanbanFlow sends a webhook to this app, the app will format a message for slack that includes who performed the action along with a link to the task that was created/modified (if the task was deleted there is no url b/c it doesn't exist anymore)
3. Once the message has been generated it is sent to the configured slack channel via the incoming webhook url.

How to configure it
===================
1. Generate an api key from KanbanFlow. This api key is used on application start-up to retrieve the user list.
2. Set the api key as the `kanbanflowApiKey` in the web.config and/or the azure configuration interface (or however you normally set your config up)
3. Configure an incoming webhook in Slack for the channel that you want. 
4. Set the incoming webhook url as the `slackWebhookUrl` in the web.config and/or the azure configuration interface (or however you normally set your config up)
5. If you only want the first name of the KanbanFlow user to show up in the Slack message the set the `reportFirstNameOnly` configuration setting to true

How to host it
=============
I'm hosting it in Azure and have the 3 configuration values above set in the Azure configuration section of the web application.

How to monitor it
==================
To view the trace logs in real time make sure you have the azure command line tools installed and run

`azure site log tail kanbanflow2slack`

How it looks
=============
![](http://i.imgur.com/Gr23Flo.png)