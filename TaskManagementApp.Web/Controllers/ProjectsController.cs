using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementApp.Domain.Interface;

namespace TaskManagementApp.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUserRepository _userRepository;

        public ProjectsController(IProjectService projectService, IUserRepository userRepository)
        {
            _projectService = projectService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        public async Task<IActionResult> Create()
        {
            var users = await _userRepository.GetAllAsync();
            ViewData["Users"] = users.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectDTO projectDto)
        {
            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectAsync(projectDto);
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }
    }
}
