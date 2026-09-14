using Microsoft.AspNetCore.Mvc;

namespace Vyzor.Web.Controllers
{
    public class ProfileController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
