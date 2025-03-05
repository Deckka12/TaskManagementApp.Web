using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApp.Web.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
