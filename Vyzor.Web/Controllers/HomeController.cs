using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}