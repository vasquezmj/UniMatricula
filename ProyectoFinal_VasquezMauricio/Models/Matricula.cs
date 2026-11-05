namespace ProyectoFinal.Models;

/// <summary>Relación entre estudiante, carrera y curso (la carrera se obtiene del estudiante y del curso).</summary>
public class Matricula
{
    public int Id { get; set; }

    public string EstudianteId { get; set; } = string.Empty;
    public ApplicationUser? Estudiante { get; set; }

    public int CursoId { get; set; }
    public Curso? Curso { get; set; }

    public DateTime FechaMatricula { get; set; } = DateTime.Now;
}
