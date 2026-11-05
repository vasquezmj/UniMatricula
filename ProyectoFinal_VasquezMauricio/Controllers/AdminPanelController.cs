using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

/// <summary>Panel de administración (HU-10, HU-16).</summary>
[Authorize(Policy = "SoloAdmin")]
[Route("Admin")]
public class AdminPanelController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminPanelController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var estudiantes = await (from u in _db.Users
                                 join ur in _db.UserRoles on u.Id equals ur.UserId
                                 join r in _db.Roles on ur.RoleId equals r.Id
                                 where r.Name == "Estudiante"
                                 select u.Id).CountAsync();

        return View(new DashboardAdminViewModel
        {
            Estudiantes = estudiantes,
            Carreras = await _db.Carreras.CountAsync(),
            Cursos = await _db.Cursos.CountAsync(),
            Docentes = await _db.Docentes.CountAsync(),
            Matriculas = await _db.Matriculas.CountAsync()
        });
    }
}
