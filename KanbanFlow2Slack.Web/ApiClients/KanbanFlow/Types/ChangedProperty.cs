namespace KanbanFlow2Slack.Web.ApiClients.KanbanFlow.Types
{
    public class ChangedProperty
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string Property { get; set; }
    }
}