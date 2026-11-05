using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<CarreraCurso> CarreraCursos => Set<CarreraCurso>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.ToTable("Estudiantes");
            e.Property(u => u.Nombre).HasMaxLength(80);
            e.Property(u => u.Apellidos).HasMaxLength(80);
            e.Property(u => u.Cedula).HasMaxLength(20);
            e.HasIndex(u => u.Cedula);
            e.HasOne(u => u.Carrera).WithMany(c => c.Estudiantes)
                .HasForeignKey(u => u.CarreraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CarreraCurso>(e =>
        {
            e.HasKey(x => new { x.CarreraId, x.CursoId });
            e.HasOne(x => x.Carrera).WithMany(c => c.CarreraCursos)
                .HasForeignKey(x => x.CarreraId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Curso).WithMany(c => c.CarreraCursos)
                .HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Curso>(e =>
        {
            e.HasIndex(c => c.Codigo).IsUnique();
            e.HasOne(c => c.Docente).WithMany(d => d.Cursos)
                .HasForeignKey(c => c.DocenteId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Docente>().HasIndex(d => d.Correo).IsUnique();

        builder.Entity<Matricula>(e =>
        {
            e.HasIndex(m => new { m.EstudianteId, m.CursoId }).IsUnique();
            e.HasOne(m => m.Estudiante).WithMany(u => u.Matriculas)
                .HasForeignKey(m => m.EstudianteId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Curso).WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
