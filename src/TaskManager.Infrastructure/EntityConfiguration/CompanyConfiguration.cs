﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Entities;

namespace TaskManager.Infrastructure.EntityConfiguration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies")
                .HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired();

            builder.HasMany(t => t.Projects)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId);

            builder.HasMany(t => t.Employees)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId);
        }
    }
}
