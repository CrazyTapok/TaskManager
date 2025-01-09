namespace TaskManager.Core.Models;

public class Company : BaseEntity
{
    public required string Title { get; set; }

    public List<Project> Projects { get; set; }

    public List<Employee> Employees { get; set; }
}
