namespace TaskManager.API.Contracts.Response;

public class ProjectResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid ManagerId { get; set; }
    public Guid CompanyId { get; set; }
}
