namespace TaskManager.Core.Models;

public class Project : BaseEntity
{
    public string Title { get; set; }

    public Guid ManagerId { get; set; }
    public Employee Manager { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; }

    public List<Employee> Employees { get; set; }

    public List<Task> Tasks { get; set; }
}
