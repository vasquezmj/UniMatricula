using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    // ---------- Registro (HU-01, HU-03) ----------
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        await CargarCarreras();
        return View(new RegisterViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!await _db.Carreras.AnyAsync(c => c.Id == model.CarreraId && c.Activa))
                ModelState.AddModelError(nameof(model.CarreraId), "La carrera seleccionada no existe.");

            if (await _db.Users.AnyAsync(u => u.Cedula == model.Cedula))
                ModelState.AddModelError(nameof(model.Cedula), "Ya existe un estudiante con esa cédula.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCarreras();
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Nombre = model.Nombre.Trim(),
            Apellidos = model.Apellidos.Trim(),
            Cedula = model.Cedula.Trim(),
            CarreraId = model.CarreraId
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Estudiante");
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Ok"] = "¡Cuenta creada con éxito! Ya puedes matricular tus cursos.";
            return RedirectToAction("Index", "Estudiante");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        await CargarCarreras();
        return View(model);
    }

    // ---------- Inicio de sesión (HU-02, HU-10) ----------
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            // Mensaje genérico: no revela cuál dato falló
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index", "AdminPanel");

        return RedirectToAction("Index", "Estudiante");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task CargarCarreras()
    {
        var carreras = await _db.Carreras.Where(c => c.Activa).OrderBy(c => c.Nombre).ToListAsync();
        ViewBag.Carreras = new SelectList(carreras, "Id", "Nombre");
    }
}
