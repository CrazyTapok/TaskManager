using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.EntityConfiguration;

internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects")
            .HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.HasMany(t => t.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
