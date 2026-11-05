using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

/// <summary>Matrícula de cursos del estudiante (HU-05, HU-06, HU-07, HU-08).</summary>
[Authorize(Policy = "SoloEstudiante")]
public class MatriculaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public MatriculaController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? creditos, string? tipo)
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _db.Users.Include(u => u.Carrera).FirstAsync(u => u.Id == userId);

        if (user.CarreraId == null)
        {
            TempData["Error"] = "Primero selecciona una carrera.";
            return RedirectToAction("Index", "Carreras");
        }

        var query = _db.Cursos
            .Where(c => c.CarreraCursos.Any(cc => cc.CarreraId == user.CarreraId))
            .Where(c => !c.Matriculas.Any(m => m.EstudianteId == userId));

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Nombre.Contains(q) || c.Codigo.Contains(q));
        if (creditos.HasValue)
            query = query.Where(c => c.Creditos == creditos.Value);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(c => c.Tipo == tipo);

        var cursos = await query.OrderBy(c => c.Codigo)
            .Select(c => new CursoDisponibleViewModel
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                Creditos = c.Creditos,
                Tipo = c.Tipo,
                Docente = c.Docente != null ? c.Docente.Nombre : null,
                CuposDisponibles = c.Cupos - c.Matriculas.Count
            }).ToListAsync();

        ViewBag.Carrera = user.Carrera!.Nombre;
        ViewBag.Q = q;
        ViewBag.Creditos = creditos;
        ViewBag.Tipo = tipo;
        return View(cursos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(int[] cursoIds)
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _db.Users.FirstAsync(u => u.Id == userId);

        if (cursoIds == null || cursoIds.Length == 0)
        {
            TempData["Error"] = "Selecciona al menos un curso para matricular.";
            return RedirectToAction(nameof(Index));
        }

        var cursos = await _db.Cursos
            .Include(c => c.Matriculas)
            .Include(c => c.CarreraCursos)
            .Where(c => cursoIds.Contains(c.Id))
            .ToListAsync();

        var errores = new List<string>();
        var nuevos = new List<Matricula>();

        foreach (var curso in cursos)
        {
            if (!curso.CarreraCursos.Any(cc => cc.CarreraId == user.CarreraId))
                errores.Add($"{curso.Codigo} no pertenece a tu carrera.");
            else if (curso.Matriculas.Any(m => m.EstudianteId == userId))
                errores.Add($"{curso.Codigo} ya está matriculado.");
            else if (curso.Cupos - curso.Matriculas.Count <= 0)
                errores.Add($"{curso.Codigo} no tiene cupos disponibles.");
            else
                nuevos.Add(new Matricula { EstudianteId = userId, CursoId = curso.Id, FechaMatricula = DateTime.Now });
        }

        if (errores.Count > 0)
        {
            TempData["Error"] = "No se pudo completar la matrícula: " + string.Join(" ", errores);
            return RedirectToAction(nameof(Index));
        }

        _db.Matriculas.AddRange(nuevos);
        await _db.SaveChangesAsync();
        TempData["Ok"] = $"¡Matrícula confirmada! Se inscribieron {nuevos.Count} curso(s).";
        return RedirectToAction(nameof(MisCursos));
    }

    public async Task<IActionResult> MisCursos()
    {
        var userId = _userManager.GetUserId(User)!;
        var matriculas = await _db.Matriculas
            .Include(m => m.Curso).ThenInclude(c => c!.Docente)
            .Where(m => m.EstudianteId == userId)
            .OrderBy(m => m.Curso!.Codigo)
            .ToListAsync();
        return View(matriculas);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Retirar(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var matricula = await _db.Matriculas.FirstOrDefaultAsync(m => m.Id == id && m.EstudianteId == userId);
        if (matricula == null) return NotFound();

        _db.Matriculas.Remove(matricula);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Curso retirado correctamente.";
        return RedirectToAction(nameof(MisCursos));
    }
}
