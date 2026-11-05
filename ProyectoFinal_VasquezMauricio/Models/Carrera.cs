using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Carrera
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;

    public ICollection<CarreraCurso> CarreraCursos { get; set; } = new List<CarreraCurso>();
    public ICollection<ApplicationUser> Estudiantes { get; set; } = new List<ApplicationUser>();
}
