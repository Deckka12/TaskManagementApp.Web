using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementApp.Application.DTOs;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain.Interface;
using TaskManagementApp.Web.Models;

public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, IProjectService projectService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _projectService = projectService;
        _logger = logger;
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
}
