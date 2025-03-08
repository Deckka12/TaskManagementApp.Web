using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Domain.Interface;
using TaskManagementApp.Infrastructure.DBContext;
using TaskManagementApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementApp.Web.Controllers
{
    [Route("api/worklog")]
    [ApiController]
    public class WorkLogController : ControllerBase
    {
        private readonly IWorkLogService _workLogService;
        private readonly AppDbContext _context;

        public WorkLogController(IWorkLogService workLogService, AppDbContext context)
        {
            _workLogService = workLogService;
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddWorkLog([FromBody] WorkLogDto workLogDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null) return Unauthorized();

            await _workLogService.AddWorkLogAsync(workLogDto.TaskId, user.Id, workLogDto.HoursSpent, workLogDto.WorkType, workLogDto.Comment);
            return Ok(new { message = "Трудозатраты добавлены" });
        }

        [HttpGet("history/{taskId}")]
        public async Task<IActionResult> GetWorkLogs(Guid taskId)
        {
            var logs = await _workLogService.GetWorkLogsAsync(taskId);
            return Ok(logs);
        }

        [HttpGet("total/{taskId}")]
        public async Task<IActionResult> GetTotalHours(Guid taskId)
        {
            var total = await _workLogService.GetTotalHoursAsync(taskId);
            return Ok(new { totalHours = total });
        }
    }

}
