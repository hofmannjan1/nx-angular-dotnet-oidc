using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
