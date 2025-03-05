using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementApp.Application.Services;
using TaskManagementApp.Domain.Interface;

public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public TasksController(ITaskService taskService, IProjectService projectService, IUserRepository userRepository,IProjectRepository projectRepository)
    {
        _taskService = taskService;
        _projectService = projectService;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return View(tasks);
    }

    public async Task<IActionResult> Create()
    {
        var users = await _userRepository.GetAllAsync();
        var projects = await _projectRepository.GetAllAsync();

        ViewData["Users"] = users.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToList();
        ViewData["Projects"] = projects.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(TaskDTO model)
    {
        if (ModelState.IsValid)
        {
            await _taskService.CreateTaskAsync(model);
            return RedirectToAction("Index");
        }
        return View(model);
    }
}
