using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers;

/// <summary>CRUD de docentes para el administrador (HU-14). La asignación a cursos se hace en el formulario del curso (HU-15).</summary>
[Authorize(Policy = "SoloAdmin")]
[Route("Admin/Docentes")]
public class AdminDocentesController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminDocentesController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var docentes = await _db.Docentes.Include(d => d.Cursos).OrderBy(d => d.Nombre).ToListAsync();
        return View(docentes);
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
        TempData["Ok"] = "Docente registrado correctamente.";
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
        TempData["Ok"] = "Docente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var docente = await _db.Docentes.FindAsync(id);
        if (docente == null) return NotFound();

        _db.Docentes.Remove(docente); // los cursos quedan sin docente (SetNull)
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Docente eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
