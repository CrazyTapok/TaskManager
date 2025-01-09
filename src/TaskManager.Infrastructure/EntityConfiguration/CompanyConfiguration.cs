using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.EntityConfiguration;

internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies")
            .HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.HasMany(t => t.Projects)
            .WithOne(t => t.Company)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(t => t.Employees)
            .WithOne(t => t.Company)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
