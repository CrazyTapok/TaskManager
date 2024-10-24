namespace TaskManager.Core.Models;

public class Project
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public Guid ManagerId { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; }

    public List<Employee> Employees { get; set; }

    public List<Task> Tasks { get; set; }
}
