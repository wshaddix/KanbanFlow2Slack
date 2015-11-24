namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    internal class ChangedProperty
    {
        internal string NewValue { get; set; }
        internal string OldValue { get; set; }
        internal string Property { get; set; }
    }
}