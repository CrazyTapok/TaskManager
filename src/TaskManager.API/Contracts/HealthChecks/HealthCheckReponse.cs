namespace TaskManager.API.Contracts.HealthChecks;

internal class HealthCheckReponse
{
    public string Status { get; set; }
    public IEnumerable<IndividualHealthCheckResponse> HealthChecks { get; set; }
    public TimeSpan HealthCheckDuration { get; set; }
}