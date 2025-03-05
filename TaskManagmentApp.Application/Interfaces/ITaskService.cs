using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApp.Application.DTOs;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDTO>> GetAllTasksAsync();
        Task<TaskDTO?> GetTaskByIdAsync(Guid id);
        Task CreateTaskAsync(TaskDTO taskDto);
        Task UpdateTaskAsync(TaskDTO taskDto);
        Task DeleteTaskAsync(Guid id);
    }
}
