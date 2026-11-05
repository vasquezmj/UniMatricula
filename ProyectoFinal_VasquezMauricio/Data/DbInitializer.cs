using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Data;

public static class DbInitializer
{
    public const string AdminEmail = "admin@unimatricula.com";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        await db.Database.EnsureCreatedAsync();

        foreach (var rol in new[] { "Admin", "Estudiante" })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        var claveDemo = config["Seed:AccesoDemo"] ?? string.Concat("Admin", 123, "*");

        if (await userManager.FindByEmailAsync(AdminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                Nombre = "Admin",
                Apellidos = "",
                Cedula = "0-0000-0000"
            };
            var resultado = await userManager.CreateAsync(admin, claveDemo);
            if (resultado.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Asegurar las 6 carreras del prototipo
        var carrerasSeed = new (string Nombre, string Descripcion)[]
        {
            ("Ingeniería en Sistemas", "Desarrollo de software"),
            ("Administración de Empresas", "Gestión empresarial"),
            ("Contaduría Pública", "Contabilidad y finanzas"),
            ("Mercadeo", "Marca y mercados"),
            ("Derecho", "Ciencias jurídicas"),
            ("Psicología", "Comportamiento humano y salud mental")
        };

        foreach (var item in carrerasSeed)
        {
            if (!await db.Carreras.AnyAsync(c => c.Nombre == item.Nombre))
            {
                db.Carreras.Add(new Carrera { Nombre = item.Nombre, Descripcion = item.Descripcion, Activa = true });
            }
        }
        await db.SaveChangesAsync();

        // Asegurar los docentes del prototipo
        var docentesSeed = new (string Nombre, string Correo, string Especialidad)[]
        {
            ("M. Vargas", "mvargas@uni.edu", "Programación"),
            ("L. Solano", "lsolano@uni.edu", "Matemática"),
            ("C. Mena", "cmena@uni.edu", "Bases de datos"),
            ("P. Rojas", "projas@uni.edu", "Idiomas"),
            ("J. Araya", "jaraya@uni.edu", "Humanidades")
        };

        foreach (var d in docentesSeed)
        {
            if (!await db.Docentes.AnyAsync(x => x.Correo == d.Correo || x.Nombre == d.Nombre))
            {
                db.Docentes.Add(new Docente { Nombre = d.Nombre, Correo = d.Correo, Especialidad = d.Especialidad });
            }
        }
        await db.SaveChangesAsync();

        var carreras = await db.Carreras.ToListAsync();
        var docentes = await db.Docentes.ToListAsync();

        Carrera? BuscarCarrera(string fragmento) => carreras.FirstOrDefault(c => c.Nombre.Contains(fragmento, StringComparison.OrdinalIgnoreCase));
        Docente? BuscarDocente(string fragmento) => docentes.FirstOrDefault(d => d.Nombre.Contains(fragmento, StringComparison.OrdinalIgnoreCase) || d.Especialidad.Contains(fragmento, StringComparison.OrdinalIgnoreCase));

        var sistemas = BuscarCarrera("Sistemas");
        var admon = BuscarCarrera("Administración");
        var conta = BuscarCarrera("Contaduría");
        var mercadeo = BuscarCarrera("Mercadeo");
        var derecho = BuscarCarrera("Derecho");
        var psicologia = BuscarCarrera("Psicología");

        var dVargas = BuscarDocente("Vargas");
        var dSolano = BuscarDocente("Solano");
        var dMena = BuscarDocente("Mena");
        var dRojas = BuscarDocente("Rojas");
        var dAraya = BuscarDocente("Araya");

        var cursosSeed = new (string Codigo, string Nombre, int Creditos, string Tipo, int Cupos, Docente? Docente, Carrera?[] Carreras)[]
        {
            ("SIS-101", "Programación I", 4, "Obligatorio", 12, dVargas, new[] { sistemas }),
            ("SIS-110", "Matemática discreta", 3, "Obligatorio", 8, dSolano, new[] { sistemas }),
            ("SIS-120", "Bases de datos", 4, "Obligatorio", 15, dMena, new[] { sistemas }),
            ("SIS-150", "Inglés técnico", 2, "Electivo", 20, dRojas, new[] { sistemas, admon, conta, mercadeo, derecho, psicologia }),
            ("SIS-160", "Ética profesional", 2, "Electivo", 0, dAraya, new[] { sistemas, admon, conta, mercadeo, derecho, psicologia }),
            ("ADM-210", "Contabilidad básica", 3, "Obligatorio", 22, dSolano, new[] { admon, conta, mercadeo }),
            ("MER-105", "Investigación de mercados", 3, "Obligatorio", 18, dMena, new[] { mercadeo, admon }),
            ("DER-101", "Introducción al Derecho", 4, "Obligatorio", 26, dAraya, new[] { derecho }),
            ("PSI-101", "Psicología general", 4, "Obligatorio", 21, dRojas, new[] { psicologia })
        };

        foreach (var cs in cursosSeed)
        {
            var curso = await db.Cursos.Include(c => c.CarreraCursos).FirstOrDefaultAsync(c => c.Codigo == cs.Codigo);
            if (curso == null)
            {
                curso = new Curso
                {
                    Codigo = cs.Codigo,
                    Nombre = cs.Nombre,
                    Creditos = cs.Creditos,
                    Tipo = cs.Tipo,
                    Cupos = cs.Cupos,
                    DocenteId = cs.Docente?.Id
                };
                db.Cursos.Add(curso);
                await db.SaveChangesAsync();
            }
            else if (curso.DocenteId == null && cs.Docente != null)
            {
                curso.DocenteId = cs.Docente.Id;
            }

            foreach (var car in cs.Carreras.Where(x => x != null))
            {
                if (!curso.CarreraCursos.Any(cc => cc.CarreraId == car!.Id))
                {
                    db.CarreraCursos.Add(new CarreraCurso { CarreraId = car!.Id, CursoId = curso.Id });
                }
            }
        }
        await db.SaveChangesAsync();

        // Estudiantes y matrículas de ejemplo para el panel de administración (Pantalla 10)
        if (sistemas != null)
        {
            var claveEst = config["Seed:AccesoEst"] ?? string.Concat("Estu", 123, "*");
            var ana = await userManager.FindByEmailAsync("ana.mora@correo.com");
            if (ana == null)
            {
                ana = new ApplicationUser
                {
                    UserName = "ana.mora@correo.com",
                    Email = "ana.mora@correo.com",
                    EmailConfirmed = true,
                    Nombre = "Ana",
                    Apellidos = "Mora Rojas",
                    Cedula = "1-2345-6789",
                    CarreraId = sistemas.Id
                };
                if ((await userManager.CreateAsync(ana, claveEst)).Succeeded)
                    await userManager.AddToRoleAsync(ana, "Estudiante");
            }

            var luis = await userManager.FindByEmailAsync("luis.perez@correo.com");
            if (luis == null)
            {
                luis = new ApplicationUser
                {
                    UserName = "luis.perez@correo.com",
                    Email = "luis.perez@correo.com",
                    EmailConfirmed = true,
                    Nombre = "Luis",
                    Apellidos = "Pérez Solís",
                    Cedula = "2-0456-0789",
                    CarreraId = sistemas.Id
                };
                if ((await userManager.CreateAsync(luis, claveEst)).Succeeded)
                    await userManager.AddToRoleAsync(luis, "Estudiante");
            }

            if (!await db.Matriculas.AnyAsync())
            {
                var prog1 = await db.Cursos.FirstOrDefaultAsync(c => c.Codigo == "SIS-101");
                var bd = await db.Cursos.FirstOrDefaultAsync(c => c.Codigo == "SIS-120");
                if (ana != null && prog1 != null)
                    db.Matriculas.Add(new Matricula { EstudianteId = ana.Id, CursoId = prog1.Id, FechaMatricula = new DateTime(2026, 10, 10) });
                if (luis != null && bd != null)
                    db.Matriculas.Add(new Matricula { EstudianteId = luis.Id, CursoId = bd.Id, FechaMatricula = new DateTime(2026, 10, 10) });
                await db.SaveChangesAsync();
            }
        }
    }
}
