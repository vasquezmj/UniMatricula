using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers;

/// <summary>CRUD de docentes y asignación a cursos (HU-14, HU-15).</summary>
[Authorize(Policy = "SoloAdmin")]
[Route("Admin/Profesores")]
[Route("Admin/Docentes")]
public class AdminDocentesController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminDocentesController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var docentes = await _db.Docentes.Include(d => d.Cursos).OrderBy(d => d.Nombre).ToListAsync();
        var carreras = await _db.Carreras.OrderBy(c => c.Nombre).ToListAsync();
        var cursos = await _db.Cursos
            .Include(c => c.Docente)
            .Include(c => c.CarreraCursos)
            .OrderBy(c => c.Codigo)
            .ToListAsync();

        ViewBag.Carreras = carreras;
        ViewBag.Cursos = cursos;
        ViewBag.AsignacionesRecientes = cursos
            .Where(c => c.Docente != null)
            .Take(4)
            .Select(c => $"{c.Codigo} → {c.Docente!.Nombre}")
            .ToList();

        return View(docentes);
    }

    [HttpPost("Asignar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int docenteId, int cursoId)
    {
        var curso = await _db.Cursos.FindAsync(cursoId);
        var docente = await _db.Docentes.FindAsync(docenteId);
        if (curso == null || docente == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest(new { ok = false, mensaje = "Selecciona un profesor y un curso válidos." });
            TempData["Error"] = "Selecciona un profesor y un curso válidos.";
            return RedirectToAction(nameof(Index));
        }

        curso.DocenteId = docente.Id;
        await _db.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var recientes = await _db.Cursos
                .Include(c => c.Docente)
                .Where(c => c.Docente != null)
                .OrderBy(c => c.Codigo)
                .Take(4)
                .Select(c => $"{c.Codigo} → {c.Docente!.Nombre}")
                .ToListAsync();
            return Json(new { ok = true, mensaje = $"Profesor {docente.Nombre} asignado a {curso.Codigo} ({curso.Nombre}).", recientes });
        }

        TempData["Ok"] = $"Profesor {docente.Nombre} asignado al curso {curso.Nombre}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new Docente());

    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Correo,Especialidad")] Docente docente)
    {
        if (await _db.Docentes.AnyAsync(d => d.Correo == docente.Correo))
            ModelState.AddModelError(nameof(docente.Correo), "Ya existe un docente con ese correo.");

        if (!ModelState.IsValid) return View(docente);

        _db.Docentes.Add(docente);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Profesor registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var docente = await _db.Docentes.FindAsync(id);
        return docente == null ? NotFound() : View(docente);
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,Especialidad")] Docente docente)
    {
        if (id != docente.Id) return NotFound();

        if (await _db.Docentes.AnyAsync(d => d.Correo == docente.Correo && d.Id != id))
            ModelState.AddModelError(nameof(docente.Correo), "Ya existe un docente con ese correo.");

        if (!ModelState.IsValid) return View(docente);

        var actual = await _db.Docentes.FindAsync(id);
        if (actual == null) return NotFound();

        actual.Nombre = docente.Nombre;
        actual.Correo = docente.Correo;
        actual.Especialidad = docente.Especialidad;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Profesor actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var docente = await _db.Docentes.FindAsync(id);
        if (docente == null) return NotFound();

        _db.Docentes.Remove(docente);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Profesor eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
