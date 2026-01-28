namespace LogHub.Core.DTOs;

public class LogStats
{
    public int TotalLogs { get; set; }
    public int TraceCount { get; set; }
    public int DebugCount { get; set; }
    public int InformationCount { get; set; }
    public int WarningCount { get; set; }
    public int ErrorCount { get; set; }
    public int CriticalCount { get; set; }
    public Dictionary<string, int> LogsByApplication { get; set; } = new();
    public Dictionary<string, int> LogsByHour { get; set; } = new();
    public Dictionary<string, int> LogsByDay { get; set; } = new();
}
