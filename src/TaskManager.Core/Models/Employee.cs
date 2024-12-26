using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

public class Employee : BaseEntity
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; }

    public List<Project> Projects { get; set; }

    public List<Project> ManagerProjects { get; set; }

    public List<Task> CreatedTasks { get; set; }

    public List<Task> AssignedTasks { get; set; }

    public Role Role { get; set; }
}