using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementApp.Application.DTOs;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interface;
using TaskManagementApp.Infrastructure.DBContext;
using TaskManagementApp.Web.Models;



public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly ILogger<TasksController> _logger;
    private readonly AppDbContext _context;
    public TasksController(ITaskService taskService, IProjectService projectService, ILogger<TasksController> logger, AppDbContext context)
    {
        _taskService = taskService;
        _projectService = projectService;
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return View(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке списка задач.");
            return View("Error", new ErrorViewModel { Message = "Не удалось загрузить задачи." });
        }
    }

    // Мои задачи
    public async Task<IActionResult> MyTasks()
    {
        // Получаем идентификатор текущего пользователя (из Claim)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Преобразуем строку в Guid (если это необходимо)
        if (Guid.TryParse(userId, out var parsedUserId))
        {
            var tasks = await _taskService.GetTasksByUser(parsedUserId); // Получаем задачи текущего пользователя
            return View("Index", tasks); // Передаем задачи в представление
        }

        // В случае ошибки (если userId не является допустимым Guid), можно вернуть ошибку или пустой список
        return View("Error", new ErrorViewModel { Message = "Невозможно получить задачи для текущего пользователя." });
    }

    public async Task<IActionResult> Create()
    {
        await PopulateProjectsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskDTO taskDto)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId) || userId == Guid.Empty)
        {
            ModelState.AddModelError("", "Ошибка определения пользователя.");
            await PopulateProjectsAsync();
            return View(taskDto);
        }

        taskDto.UserId = userId;
        taskDto.workLogs = null;

        if (!ModelState.IsValid)
        {
            await PopulateProjectsAsync();
            return View(taskDto);
        }

        try
        {
            var project = await _projectService.GetProjectByIdAsync(taskDto.ProjectId);
            if (project == null)
            {
                ModelState.AddModelError("ProjectId", "Выбранный проект не существует.");
                await PopulateProjectsAsync();
                return View(taskDto);
            }

            await _taskService.CreateTaskAsync(taskDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании задачи.");
            ModelState.AddModelError("", "Не удалось создать задачу.");
            await PopulateProjectsAsync();
            return View(taskDto);
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке задачи с ID: {TaskId}", id);
            return View("Error", new ErrorViewModel { Message = "Не удалось загрузить задачу." });
        }
    }

    private async Task PopulateProjectsAsync()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        ViewData["Projects"] = projects.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name
        }).ToList();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return Json(new { success = false, message = "Задача не найдена" });
        }

        await _taskService.DeleteTaskAsync(id);

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddWorkLog(WorkLogDto workLogDto)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", new { id = workLogDto.TaskId });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var workLog = new WorkLog
        {
            TaskId = workLogDto.TaskId,
            UserId = Guid.Parse(userId),
            HoursSpent = workLogDto.HoursSpent,
            WorkType = workLogDto.WorkType,
            Comment = workLogDto.Comment,
            Date = DateTime.UtcNow
        };

        _context.WorkLogs.Add(workLog);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = workLogDto.TaskId });
    }



    [HttpGet]
    public async Task<IActionResult> GetTotalWorkLog(Guid id)
    {
        var totalHours = await _context.WorkLogs
            .Where(w => w.TaskId == id)
            .SumAsync(w => w.HoursSpent);

        return Ok(new { totalHours });
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkLogHistory(Guid id)
    {
        var workLogs = await _context.WorkLogs
            .Where(w => w.TaskId == id)
            .OrderByDescending(w => w.Date)
            .Select(w => new
            {
                Date = w.Date.ToString("yyyy-MM-dd HH:mm"),
                w.HoursSpent,
                w.WorkType,
                w.Comment
            })
            .ToListAsync();

        return Ok(workLogs);
    }

    // GET: Tasks/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        // Заполнение данных для редактирования
        return View(task);
    }

    // POST: Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TaskDTO taskDto)
    {
        if (id != taskDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Обновление задачи
                await _taskService.UpdateTaskAsync(taskDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(taskDto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(taskDto);
    }

    // Проверка на существование задачи
    private bool TaskExists(Guid id)
    {
        return _taskService.GetTaskByIdAsync(id) != null;
    }



}
