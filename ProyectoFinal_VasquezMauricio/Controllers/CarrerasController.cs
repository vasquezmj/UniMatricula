using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

/// <summary>Carreras disponibles para el estudiante (HU-03, HU-04).</summary>
[Authorize(Policy = "SoloEstudiante")]
public class CarrerasController : Controller
{
    private const int TamanoPagina = 6;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public CarrerasController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int pagina = 1)
    {
        var userId = _userManager.GetUserId(User)!;
        var carreraActual = await _db.Users.Where(u => u.Id == userId).Select(u => u.CarreraId).FirstAsync();

        var query = _db.Carreras.Where(c => c.Activa);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Nombre.Contains(q));

        var total = await query.CountAsync();
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanoPagina));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var items = await query.OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * TamanoPagina).Take(TamanoPagina)
            .Select(c => new CarreraItemViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                CantidadCursos = c.CarreraCursos.Count,
                Seleccionada = c.Id == carreraActual
            }).ToListAsync();

        ViewBag.Q = q;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Seleccionar(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _db.Users.FirstAsync(u => u.Id == userId);

        if (!await _db.Carreras.AnyAsync(c => c.Id == id && c.Activa))
        {
            TempData["Error"] = "La carrera seleccionada no existe.";
            return RedirectToAction(nameof(Index));
        }

        if (user.CarreraId != id && await _db.Matriculas.AnyAsync(m => m.EstudianteId == userId))
        {
            TempData["Error"] = "No puedes cambiar de carrera mientras tengas cursos matriculados. Retira tus cursos primero.";
            return RedirectToAction(nameof(Index));
        }

        user.CarreraId = id;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Carrera seleccionada. Ya puedes ver sus cursos en Matrícula.";
        return RedirectToAction(nameof(Index));
    }
}
