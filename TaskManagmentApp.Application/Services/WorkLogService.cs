using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interface;

namespace TaskManagementApp.Application.Services
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IWorkLogRepository _workLogRepository;

        public WorkLogService(IWorkLogRepository workLogRepository)
        {
            _workLogRepository = workLogRepository;
        }

        public async Task AddWorkLogAsync(Guid taskId, Guid userId, double hours, string workType, string comment)
        {
            var workLog = new WorkLog
            {
                TaskId = taskId,
                UserId = userId,
                HoursSpent = hours,
                WorkType = workType,
                Comment = comment,
                Date = DateTime.UtcNow
            };

            await _workLogRepository.AddAsync(workLog);
        }

        public async Task<List<WorkLog>> GetWorkLogsAsync(Guid taskId)
        {
            return await _workLogRepository.GetByTaskIdAsync(taskId);
        }

        public async Task<double> GetTotalHoursAsync(Guid taskId)
        {
            return await _workLogRepository.GetTotalHoursAsync(taskId);
        }
    }

}
