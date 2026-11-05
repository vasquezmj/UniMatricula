using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(80, ErrorMessage = "Máximo 80 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(80, ErrorMessage = "Máximo 80 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cédula es obligatoria")]
    [RegularExpression(@"^[0-9\-]{9,15}$", ErrorMessage = "Cédula inválida (solo números y guiones)")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Debe incluir una mayúscula y un número")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona una carrera")]
    [Display(Name = "Carrera")]
    public int? CarreraId { get; set; }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}

public class EstudiantePanelViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Carrera { get; set; }
    public int Creditos { get; set; }
    public int Cursos { get; set; }
    public string Periodo { get; set; } = "III-2026";
}

public class CarreraItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CantidadCursos { get; set; }
    public bool Seleccionada { get; set; }
    public string Inicial => string.IsNullOrWhiteSpace(Nombre) ? "U" : Nombre.Trim()[0].ToString().ToUpper();
}

public class CursoDisponibleViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Docente { get; set; }
    public int CuposDisponibles { get; set; }
}

public class CursoFormViewModel
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

    [Display(Name = "Docente")]
    public int? DocenteId { get; set; }

    [Display(Name = "Carreras")]
    public List<int> CarreraIds { get; set; } = new();
}

public class UltimaMatriculaViewModel
{
    public string Estudiante { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}

public class DashboardAdminViewModel
{
    public int Estudiantes { get; set; }
    public int Carreras { get; set; }
    public int Cursos { get; set; }
    public int Docentes { get; set; }
    public int Matriculas { get; set; }
    public List<UltimaMatriculaViewModel> UltimasMatriculas { get; set; } = new();
}
