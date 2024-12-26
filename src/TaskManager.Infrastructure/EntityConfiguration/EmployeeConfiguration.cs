using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.EntityConfiguration;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees")
            .HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.HasIndex(t => t.Email)
            .IsUnique();

        builder.Property(t => t.Password)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasMany(t => t.Projects)
          .WithMany(t => t.Employees)
          .UsingEntity<Dictionary<string, string>>(
            "Employees_Projects",
            t => t.HasOne<Project>().WithMany().OnDelete(DeleteBehavior.NoAction),
            t => t.HasOne<Employee>().WithMany().OnDelete(DeleteBehavior.NoAction)
          );

        builder.HasMany(t => t.CreatedTasks)
            .WithOne(t => t.CreateEmployee)
            .HasForeignKey(t => t.CreateEmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(t => t.AssignedTasks)
           .WithOne(t => t.AssignedEmployee)
           .HasForeignKey(t => t.AssignedEmployeeId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(t => t.ManagerProjects)
            .WithOne(t => t.Manager)
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(t => t.Role)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
