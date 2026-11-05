# Documentación — Avance 2 Proyecto Final (UniMatrícula)

- **Estudiante:** Mauricio Vásquez Loría
- **Identificación:** 305220648
- **Curso:** SC-601 Programación Avanzada
- **Repositorio GitHub:** https://github.com/vasquezmj/UniMatricula.git

## Contenido del Entregable ZIP

1. **`/Proyecto`**: Solución completa de Visual Studio (`ProyectoFinal_VasquezMauricio`) con el código fuente ASP.NET Core MVC, Entity Framework Core y ASP.NET Core Identity.
2. **`/BaseDatos`**: Script SQL completo (`UniMatricula_BaseDatos.sql`) con la definición de tablas, llaves foráneas, índices y datos iniciales.
3. **`/Documentación`**: Guía de ejecución, credenciales de prueba y trazabilidad de historias de usuario implementadas.

## Trazabilidad de Historias de Usuario Implementadas

| Historia | Descripción | Ruta / Controlador | Estado |
| :--- | :--- | :--- | :--- |
| **HU-01** | Registro de estudiante con validaciones | `/Account/Register` (`AccountController`) | Implementada |
| **HU-02** | Inicio y cierre de sesión con Identity | `/Account/Login` (`AccountController`) | Implementada |
| **HU-03** | Selección de carrera en registro y catálogo | `/Account/Register`, `/Carreras` (`CarrerasController`) | Implementada |
| **HU-04** | Listado de carreras con búsqueda y paginación | `/Carreras` (`CarrerasController`) | Implementada |
| **HU-05** | Visualizar cursos disponibles de la carrera | `/Matricula` (`MatriculaController`) | Implementada |
| **HU-06** | Filtrar cursos por nombre, créditos o tipo (AJAX) | `/Matricula` (`MatriculaController`) | Implementada |
| **HU-07** | Resumen dinámico y confirmación de matrícula | `/Matricula`, `/Matricula/Confirmar` | Implementada |
| **HU-08** | Consultar y retirar cursos matriculados | `/Matricula/MisCursos` (`MatriculaController`) | Implementada |
| **HU-09** | Panel principal del estudiante | `/Estudiante/Inicio` (`EstudianteController`) | Implementada |
| **HU-10** | Control de acceso por rol Administrador | `[Authorize(Policy = "SoloAdmin")]` | Implementada |
| **HU-11** | CRUD de carreras con formulario lateral AJAX | `/Admin/Carreras` (`AdminCarrerasController`) | Implementada |
| **HU-12** | CRUD de cursos con filtros y paginación | `/Admin/Cursos` (`AdminCursosController`) | Implementada |
| **HU-13** | Asociar y desasociar cursos a carreras (AJAX) | `/Admin/Cursos` (`AdminCursosController`) | Implementada |
| **HU-14** | CRUD de docentes/profesores | `/Admin/Profesores` (`AdminDocentesController`) | Implementada |
| **HU-15** | Asignar profesores a cursos por carrera | `/Admin/Profesores` (`AdminDocentesController`) | Implementada |
| **HU-16** | Panel de administración con totales y últimas matrículas | `/Admin` (`AdminPanelController`) | Implementada |
