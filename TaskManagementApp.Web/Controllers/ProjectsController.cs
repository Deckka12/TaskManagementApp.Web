using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApp.Web.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
