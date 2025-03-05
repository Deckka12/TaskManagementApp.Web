﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagmentApp.Domain.Entities;

namespace TaskManagmentApp.Infastructure.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Text).IsRequired().HasMaxLength(500);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            builder.HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);
        }
    }
}
