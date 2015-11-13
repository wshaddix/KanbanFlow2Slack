using System.Collections.Generic;

internal static class Globals
{
    public static string KanbanFlowUrlTemplate = @"https://kanbanflow.com/t/{0}";
    internal static Dictionary<string, string> Users { get; set; }
}