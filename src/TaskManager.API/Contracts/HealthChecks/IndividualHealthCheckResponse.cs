namespace TaskManager.API.Contracts.HealthChecks;

internal class IndividualHealthCheckResponse
{
    public string? Status { get; set; }
    public string? Component { get; set; }
    public string? Description { get; set; }
}
