namespace TaskManager.API.Contracts.Response;
using TaskStatus = Core.Enums.TaskStatus;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CreateEmployeeId { get; set; }
    public Guid AssignedEmployeeId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public byte Image { get; set; }
}
