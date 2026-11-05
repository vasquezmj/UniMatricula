namespace ProyectoFinal.Models;

/// <summary>Relación muchos a muchos: un curso puede pertenecer a varias carreras.</summary>
public class CarreraCurso
{
    public int CarreraId { get; set; }
    public Carrera? Carrera { get; set; }

    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
}
