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

        if (await userManager.FindByEmailAsync(AdminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                Nombre = "Administrador",
                Apellidos = "del Sistema",
                Cedula = "0-0000-0000"
            };
            var claveInicial = config["Seed:AccesoDemo"] ?? string.Concat("Admin", 123, "*");
            var resultado = await userManager.CreateAsync(admin, claveInicial);
            if (resultado.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (await db.Carreras.AnyAsync()) return;

        var sistemas = new Carrera { Nombre = "Ingeniería en Sistemas", Descripcion = "Diseño y desarrollo de software y sistemas de información." };
        var admon = new Carrera { Nombre = "Administración de Empresas", Descripcion = "Gestión de organizaciones, finanzas y mercadeo." };
        var conta = new Carrera { Nombre = "Contaduría Pública", Descripcion = "Contabilidad, auditoría y finanzas." };

        var d1 = new Docente { Nombre = "Marta Vargas", Correo = "mvargas@unimatricula.com", Especialidad = "Programación" };
        var d2 = new Docente { Nombre = "Luis Solano", Correo = "lsolano@unimatricula.com", Especialidad = "Matemática" };
        var d3 = new Docente { Nombre = "Carla Mena", Correo = "cmena@unimatricula.com", Especialidad = "Bases de datos" };

        var c1 = new Curso { Codigo = "SIS-101", Nombre = "Programación I", Creditos = 4, Tipo = "Obligatorio", Cupos = 30, Docente = d1 };
        var c2 = new Curso { Codigo = "SIS-110", Nombre = "Matemática discreta", Creditos = 3, Tipo = "Obligatorio", Cupos = 25, Docente = d2 };
        var c3 = new Curso { Codigo = "SIS-120", Nombre = "Bases de datos", Creditos = 4, Tipo = "Obligatorio", Cupos = 25, Docente = d3 };
        var c4 = new Curso { Codigo = "ADM-210", Nombre = "Contabilidad básica", Creditos = 3, Tipo = "Obligatorio", Cupos = 40 };
        var c5 = new Curso { Codigo = "GEN-050", Nombre = "Inglés técnico", Creditos = 2, Tipo = "Electivo", Cupos = 40 };
        var c6 = new Curso { Codigo = "GEN-060", Nombre = "Ética profesional", Creditos = 2, Tipo = "Electivo", Cupos = 0 };

        db.Carreras.AddRange(sistemas, admon, conta);
        db.Docentes.AddRange(d1, d2, d3);
        db.Cursos.AddRange(c1, c2, c3, c4, c5, c6);
        db.CarreraCursos.AddRange(
            new CarreraCurso { Carrera = sistemas, Curso = c1 },
            new CarreraCurso { Carrera = sistemas, Curso = c2 },
            new CarreraCurso { Carrera = sistemas, Curso = c3 },
            new CarreraCurso { Carrera = sistemas, Curso = c5 },
            new CarreraCurso { Carrera = sistemas, Curso = c6 },
            new CarreraCurso { Carrera = admon, Curso = c4 },
            new CarreraCurso { Carrera = admon, Curso = c5 },
            new CarreraCurso { Carrera = conta, Curso = c4 },
            new CarreraCurso { Carrera = conta, Curso = c5 });
        await db.SaveChangesAsync();
    }
}
