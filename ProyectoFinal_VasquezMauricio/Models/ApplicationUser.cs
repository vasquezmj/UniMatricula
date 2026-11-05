using Microsoft.AspNetCore.Identity;

namespace ProyectoFinal.Models;

/// <summary>Estudiante (y también administrador): usuario de ASP.NET Identity. Tabla "Estudiantes".</summary>
public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;

    public int? CarreraId { get; set; }
    public Carrera? Carrera { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
}
