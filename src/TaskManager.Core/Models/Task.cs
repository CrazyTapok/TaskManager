using TaskManager.Core.Enums;

namespace TaskManager.Core.Models
{
    public class Task
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public ETaskStatus Status { get; set; }

        public Guid CreateEmployeeId { get; set; }

        public Guid AssinedEmployeeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public byte Image { get; set; }
    }
}
