
namespace TaskManager.Infrastructure.Entities
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<Project> Projects { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
