using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

[Authorize(Policy = "SoloEstudiante")]
public class EstudianteController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public EstudianteController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    // Panel principal del estudiante (HU-09)
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _db.Users.Include(u => u.Carrera).FirstAsync(u => u.Id == userId);

        var matriculas = await _db.Matriculas
            .Where(m => m.EstudianteId == userId)
            .Select(m => m.Curso!.Creditos)
            .ToListAsync();

        return View(new EstudiantePanelViewModel
        {
            NombreCompleto = user.NombreCompleto,
            Carrera = user.Carrera?.Nombre,
            Creditos = matriculas.Sum(),
            Cursos = matriculas.Count
        });
    }
}
