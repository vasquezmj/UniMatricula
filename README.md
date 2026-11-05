# 🎓 UniMatrícula — Sistema de Matrícula Universitaria

**Curso:** SC-601 Programación Avanzada · Universidad Fidélitas  
**Profesor:** Lenin Ortega Jiménez  
**Estudiante:** Mauricio Vásquez Loría  
**Identificación:** 305220648  
**Entregable:** Avance 2 — Proyecto Final  

---

## 📋 Descripción del Proyecto

**UniMatrícula** es un sistema funcional de matrícula universitaria desarrollado con **ASP.NET Core MVC**, **Entity Framework Core (SQL Server)** y **ASP.NET Core Identity**. Permite gestionar de manera integral estudiantes, carreras, cursos, docentes y matrículas, diferenciando el flujo e interfaz según el rol del usuario (**Estudiante** o **Administrador**) y siguiendo fielmente el prototipo definido en el Avance 1.

---

## 🚀 Tecnologías Utilizadas

* **Framework Backend / Web:** ASP.NET Core MVC (.NET 10.0)
* **ORM y Acceso a Datos:** Entity Framework Core (Code First + SQL Server / LocalDB)
* **Seguridad y Autenticación:** ASP.NET Core Identity con roles (`Admin`, `Estudiante`) y políticas de autorización (`SoloAdmin`, `SoloEstudiante`)
* **Frontend:** Razor Views, Bootstrap 5.3, jQuery, jQuery Unobtrusive Validation y `fetch` AJAX

---

## 🗄️ Estructura de Base de Datos

El modelo relacional en [`ApplicationDbContext.cs`](ProyectoFinal_VasquezMauricio/Data/ApplicationDbContext.cs) incluye las tablas principales solicitadas:

1. **`Estudiantes`** (`ApplicationUser` extendiendo `IdentityUser`): Nombre, Apellidos, Cédula única, Correo electrónico y relación con `Carrera`.
2. **`Carreras`**: Nombre, Descripción, Estado activa/inactiva y relación con estudiantes y cursos.
3. **`Cursos`**: Código único, Nombre, Créditos, Tipo (`Obligatorio` / `Electivo`), Cupos y Docente asignado.
4. **`Docentes`**: Nombre, Correo único, Especialidad y cursos a cargo.
5. **`Matriculas`**: Relación entre estudiante y curso con índice compuesto único `(EstudianteId, CursoId)` y fecha de inscripción.
6. **`CarreraCursos`**: Tabla intermedia muchos-a-muchos para asociar cursos a una o varias carreras.

El script completo de creación y datos semilla se encuentra en [`BaseDatos/UniMatricula_BaseDatos.sql`](BaseDatos/UniMatricula_BaseDatos.sql). Además, al iniciar la aplicación, [`DbInitializer.cs`](ProyectoFinal_VasquezMauricio/Data/DbInitializer.cs) crea la base de datos automáticamente e inserta los datos iniciales si no existen.

---

## 🖥️ Módulos y Pantallas Implementadas

### Rol Estudiante
* **Inicio de sesión (`/Account/Login`):** Diseño dividido con panel institucional y redirección automática según el rol (HU-02, HU-10).
* **Registro de estudiante (`/Account/Register`):** Validación en cliente y servidor con selección de carrera desde el registro (HU-01, HU-03).
* **Panel del estudiante (`/Estudiante/Inicio`):** Resumen de carrera activa, créditos matriculados, cursos inscritos y periodo `III-2026` (HU-09).
* **Selección de carrera (`/Carreras`):** Tarjetas con inicial circular, cantidad de cursos, buscador y paginación (HU-03, HU-04).
* **Matrícula de cursos (`/Matricula`):** Filtros dinámicos por AJAX (búsqueda, créditos y tipo), validación de cupos y panel lateral **Resumen** en vivo con cálculo de créditos (HU-05, HU-06, HU-07).
* **Mis cursos (`/Matricula/MisCursos`):** Confirmación de matrícula, listado de cursos inscritos y opción para retirar cursos (HU-07, HU-08).

### Rol Administrador
* **Panel de administración (`/Admin`):** Indicadores globales (Estudiantes, Carreras, Cursos, Docentes, Matrículas), accesos rápidos y tabla de **Últimas matrículas** (HU-10, HU-16).
* **Gestión de carreras (`/Admin/Carreras`):** Tabla y formulario lateral (**Nueva carrera / Editar carrera**) con actualización mediante AJAX y protección contra borrado de carreras con estudiantes (HU-11).
* **Gestión de cursos (`/Admin/Cursos`):** CRUD completo con filtros, paginación y panel lateral **Asociar cursos a una carrera** mediante AJAX (HU-12, HU-13).
* **Gestión de profesores (`/Admin/Profesores`):** CRUD de docentes y panel lateral **Asignar profesor** a cursos por carrera con historial de asignaciones recientes (HU-14, HU-15).

---

## 🔑 Cuentas de Prueba (Semilla Automática)

Al ejecutar el proyecto por primera vez, el sistema carga automáticamente las siguientes cuentas de prueba:

| Rol | Correo electrónico | Clave de acceso |
| :--- | :--- | :--- |
| **Administrador** | `admin@unimatricula.com` | `Admin` seguido de `123*` |
| **Estudiante (Demo)** | `ana.mora@correo.com` | `Estu` seguido de `123*` |

*(También puedes registrar cualquier estudiante nuevo desde `/Account/Register`).*

---

## ⚙️ Instrucciones de Ejecución

1. Clonar el repositorio o descomprimir la carpeta `/Proyecto`.
2. Abrir la solución `ProyectoFinal_VasquezMauricio.slnx` en **Visual Studio 2022 / 2026** (o ejecutar desde terminal con .NET SDK).
3. Ejecutar el proyecto:
   ```bash
   dotnet run --project ProyectoFinal_VasquezMauricio/ProyectoFinal_VasquezMauricio.csproj
   ```
4. La base de datos en **SQL Server LocalDB** se creará e inicializará automáticamente al arrancar la aplicación.
