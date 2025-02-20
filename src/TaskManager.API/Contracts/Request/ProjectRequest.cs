namespace TaskManager.API.Contracts.Request;

public class ProjectRequest
{
    public string Title { get; set; }
    public Guid ManagerId { get; set; }
    public Guid CompanyId { get; set; }
}
