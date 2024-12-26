using TaskStatus = TaskManager.Core.Enums.TaskStatus;

namespace TaskManager.Core.Models;

public class Task : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public TaskStatus Status { get; set; }

    public Guid CreateEmployeeId { get; set; }
    public Employee CreateEmployee { get; set; }

    public Guid AssignedEmployeeId { get; set; }
    public Employee AssignedEmployee { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public byte Image { get; set; }
}
