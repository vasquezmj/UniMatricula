using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers;

/// <summary>CRUD de carreras para el administrador (HU-11).</summary>
[Authorize(Policy = "SoloAdmin")]
[Route("Admin/Carreras")]
public class AdminCarrerasController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminCarrerasController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var carreras = await _db.Carreras
            .Include(c => c.CarreraCursos)
            .Include(c => c.Estudiantes)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        return View(carreras);
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new Carrera());

    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activa")] Carrera carrera)
    {
        if (await _db.Carreras.AnyAsync(c => c.Nombre == carrera.Nombre))
            ModelState.AddModelError(nameof(carrera.Nombre), "Ya existe una carrera con ese nombre.");

        if (!ModelState.IsValid) return View(carrera);

        _db.Carreras.Add(carrera);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Carrera creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var carrera = await _db.Carreras.FindAsync(id);
        return carrera == null ? NotFound() : View(carrera);
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Activa")] Carrera carrera)
    {
        if (id != carrera.Id) return NotFound();

        if (await _db.Carreras.AnyAsync(c => c.Nombre == carrera.Nombre && c.Id != id))
            ModelState.AddModelError(nameof(carrera.Nombre), "Ya existe una carrera con ese nombre.");

        if (!ModelState.IsValid) return View(carrera);

        var actual = await _db.Carreras.FindAsync(id);
        if (actual == null) return NotFound();

        actual.Nombre = carrera.Nombre;
        actual.Descripcion = carrera.Descripcion;
        actual.Activa = carrera.Activa;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Carrera actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var carrera = await _db.Carreras.Include(c => c.Estudiantes).FirstOrDefaultAsync(c => c.Id == id);
        if (carrera == null) return NotFound();

        if (carrera.Estudiantes.Count > 0)
        {
            TempData["Error"] = "No se puede eliminar una carrera con estudiantes asociados. Puedes desactivarla en su lugar.";
            return RedirectToAction(nameof(Index));
        }

        _db.Carreras.Remove(carrera);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Carrera eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
