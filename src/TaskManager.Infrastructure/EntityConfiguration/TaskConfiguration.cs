using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Task = TaskManager.Infrastructure.Entities.Task;

namespace TaskManager.Infrastructure.EntityConfiguration;

internal class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("Tasks")
            .HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.Property(t => t.Description);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.UpdatedDate)
            .HasColumnType("datetime");
    }
}
