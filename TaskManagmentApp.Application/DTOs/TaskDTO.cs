using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApp.Domain.Enums;
using TaskStatus = TaskManagementApp.Domain.Enums.TaskStatus;

namespace TaskManagementApp.Application.DTOs
{
    public class TaskDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
