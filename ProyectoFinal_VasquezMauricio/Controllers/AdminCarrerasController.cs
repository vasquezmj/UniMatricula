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

    [HttpGet("ListaJson")]
    public async Task<IActionResult> ListaJson()
    {
        var items = await _db.Carreras
            .OrderBy(c => c.Nombre)
            .Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre,
                descripcion = c.Descripcion,
                cursos = c.CarreraCursos.Count,
                estudiantes = c.Estudiantes.Count,
                activa = c.Activa
            })
            .ToListAsync();
        return Json(items);
    }

    [HttpPost("GuardarAjax"), ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarAjax([FromForm] int id, [FromForm] string nombre, [FromForm] string descripcion)
    {
        nombre = (nombre ?? string.Empty).Trim();
        descripcion = (descripcion ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion))
            return BadRequest(new { ok = false, mensaje = "El nombre y la descripción son obligatorios." });

        if (await _db.Carreras.AnyAsync(c => c.Nombre == nombre && c.Id != id))
            return BadRequest(new { ok = false, mensaje = "Ya existe una carrera con ese nombre." });

        if (id == 0)
        {
            var nueva = new Carrera { Nombre = nombre, Descripcion = descripcion, Activa = true };
            _db.Carreras.Add(nueva);
            await _db.SaveChangesAsync();
            return Json(new { ok = true, mensaje = "Carrera creada correctamente." });
        }
        else
        {
            var actual = await _db.Carreras.FindAsync(id);
            if (actual == null) return NotFound(new { ok = false, mensaje = "Carrera no encontrada." });
            actual.Nombre = nombre;
            actual.Descripcion = descripcion;
            await _db.SaveChangesAsync();
            return Json(new { ok = true, mensaje = "Carrera actualizada correctamente." });
        }
    }

    [HttpPost("EliminarAjax/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAjax(int id)
    {
        var carrera = await _db.Carreras.Include(c => c.Estudiantes).FirstOrDefaultAsync(c => c.Id == id);
        if (carrera == null) return NotFound(new { ok = false, mensaje = "Carrera no encontrada." });

        if (carrera.Estudiantes.Count > 0)
            return BadRequest(new { ok = false, mensaje = "Una carrera con estudiantes no puede eliminarse." });

        _db.Carreras.Remove(carrera);
        await _db.SaveChangesAsync();
        return Json(new { ok = true, mensaje = "Carrera eliminada correctamente." });
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
