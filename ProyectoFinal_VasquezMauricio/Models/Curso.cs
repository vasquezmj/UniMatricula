using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120, ErrorMessage = "Máximo 120 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 12, ErrorMessage = "Los créditos deben estar entre 1 y 12")]
    [Display(Name = "Créditos")]
    public int Creditos { get; set; } = 3;

    [Required(ErrorMessage = "El tipo es obligatorio")]
    public string Tipo { get; set; } = "Obligatorio";

    [Range(0, 500, ErrorMessage = "Los cupos deben estar entre 0 y 500")]
    public int Cupos { get; set; } = 30;

    public int? DocenteId { get; set; }
    public Docente? Docente { get; set; }

    public ICollection<CarreraCurso> CarreraCursos { get; set; } = new List<CarreraCurso>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
