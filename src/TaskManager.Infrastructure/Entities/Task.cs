using TaskStatus = TaskManager.Core.Enums.TaskStatus;

namespace TaskManager.Infrastructure.Entities;

public class Task
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public TaskStatus Status { get; set; }

    public Guid CreateEmployeeId { get; set; }
    public Employee CreateEmployee { get; set; }

    public Guid AssinedEmployeeId { get; set; }
    public Employee AssinedEmployee { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public byte Image { get; set; }
}
