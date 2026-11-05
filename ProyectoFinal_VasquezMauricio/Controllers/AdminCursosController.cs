using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Models.ViewModels;

namespace ProyectoFinal.Controllers;

/// <summary>CRUD de cursos, asociación a carreras y asignación de docente (HU-12, HU-13, HU-15).</summary>
[Authorize(Policy = "SoloAdmin")]
[Route("Admin/Cursos")]
public class AdminCursosController : Controller
{
    private const int TamanoPagina = 5;
    private readonly ApplicationDbContext _db;
    public AdminCursosController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index(string? q, int? creditos, string? tipo, int pagina = 1)
    {
        var query = _db.Cursos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Nombre.Contains(q) || c.Codigo.Contains(q));
        if (creditos.HasValue)
            query = query.Where(c => c.Creditos == creditos.Value);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(c => c.Tipo == tipo);

        var total = await query.CountAsync();
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanoPagina));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var cursos = await query
            .Include(c => c.Docente)
            .Include(c => c.CarreraCursos).ThenInclude(cc => cc.Carrera)
            .OrderBy(c => c.Codigo)
            .Skip((pagina - 1) * TamanoPagina).Take(TamanoPagina)
            .ToListAsync();

        ViewBag.Q = q;
        ViewBag.Creditos = creditos;
        ViewBag.Tipo = tipo;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        return View(cursos);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View(new CursoFormViewModel());
    }

    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CursoFormViewModel model)
    {
        if (await _db.Cursos.AnyAsync(c => c.Codigo == model.Codigo))
            ModelState.AddModelError(nameof(model.Codigo), "Ya existe un curso con ese código.");

        if (!ModelState.IsValid)
        {
            await CargarListas();
            return View(model);
        }

        var curso = new Curso
        {
            Codigo = model.Codigo.Trim().ToUpper(),
            Nombre = model.Nombre.Trim(),
            Creditos = model.Creditos,
            Tipo = model.Tipo,
            Cupos = model.Cupos,
            DocenteId = model.DocenteId,
            CarreraCursos = model.CarreraIds.Distinct().Select(id => new CarreraCurso { CarreraId = id }).ToList()
        };
        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Curso creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var curso = await _db.Cursos.Include(c => c.CarreraCursos).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        await CargarListas();
        return View(new CursoFormViewModel
        {
            Id = curso.Id,
            Codigo = curso.Codigo,
            Nombre = curso.Nombre,
            Creditos = curso.Creditos,
            Tipo = curso.Tipo,
            Cupos = curso.Cupos,
            DocenteId = curso.DocenteId,
            CarreraIds = curso.CarreraCursos.Select(cc => cc.CarreraId).ToList()
        });
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CursoFormViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (await _db.Cursos.AnyAsync(c => c.Codigo == model.Codigo && c.Id != id))
            ModelState.AddModelError(nameof(model.Codigo), "Ya existe un curso con ese código.");

        if (!ModelState.IsValid)
        {
            await CargarListas();
            return View(model);
        }

        var curso = await _db.Cursos.Include(c => c.CarreraCursos).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        curso.Codigo = model.Codigo.Trim().ToUpper();
        curso.Nombre = model.Nombre.Trim();
        curso.Creditos = model.Creditos;
        curso.Tipo = model.Tipo;
        curso.Cupos = model.Cupos;
        curso.DocenteId = model.DocenteId;

        // Sincroniza las carreras asociadas
        var seleccionadas = model.CarreraIds.Distinct().ToHashSet();
        foreach (var sobrante in curso.CarreraCursos.Where(cc => !seleccionadas.Contains(cc.CarreraId)).ToList())
            curso.CarreraCursos.Remove(sobrante);
        foreach (var carreraId in seleccionadas.Where(cid => curso.CarreraCursos.All(cc => cc.CarreraId != cid)))
            curso.CarreraCursos.Add(new CarreraCurso { CarreraId = carreraId, CursoId = curso.Id });

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Curso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var curso = await _db.Cursos.Include(c => c.Matriculas).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        if (curso.Matriculas.Count > 0)
        {
            TempData["Error"] = "No se puede eliminar un curso que tiene estudiantes matriculados.";
            return RedirectToAction(nameof(Index));
        }

        _db.Cursos.Remove(curso);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Curso eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListas()
    {
        ViewBag.Carreras = await _db.Carreras.OrderBy(c => c.Nombre).ToListAsync();
        ViewBag.Docentes = new SelectList(await _db.Docentes.OrderBy(d => d.Nombre).ToListAsync(), "Id", "Nombre");
        ViewBag.Tipos = new SelectList(new[] { "Obligatorio", "Electivo" });
    }
}
