using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.DTOs;

public class TasksController : Controller
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return View(tasks);
    }

    public IActionResult Create()
    {
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
