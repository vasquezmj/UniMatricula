using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Login", "Account");

        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "AdminPanel");

        return RedirectToAction("Index", "Estudiante");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
