﻿using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApp.Web.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
