namespace LogHub.Core.Constants;

public static class QueueNames
{
    public const string LogIngestion = "loghub.logs.ingestion";
    public const string LogProcessing = "loghub.logs.processing";
    public const string Alerts = "loghub.alerts";
    public const string DeadLetter = "loghub.deadletter";
}
