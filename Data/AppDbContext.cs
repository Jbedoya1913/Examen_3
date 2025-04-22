using Microsoft.EntityFrameworkCore;
using Universidad.Matriculas.Models;

namespace Universidad.Matriculas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Matricula>()
                .Property(m => m.ValorCredito)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Matricula>()
                .Property(m => m.Total)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
} 