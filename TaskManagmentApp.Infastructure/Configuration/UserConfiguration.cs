﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Infrastructure.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasMany(u => u.Projects)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId);

            builder.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
