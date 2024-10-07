using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public List<Project> Projects { get; set; }

        public List<Task> Tasks { get; set; }

        public ERole Role { get; set; }
    }
}
