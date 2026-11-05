using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Docente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Especialidad { get; set; } = string.Empty;

    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}
