using Microsoft.EntityFrameworkCore;
using Pindi.Api.Models;

namespace Pindi.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> lecturers { get; set; }
        public DbSet<DegreeProgram> DegreePrograms { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AcademicYear>().HasIndex(i => i.Year).IsUnique(true);
            modelBuilder.Entity<Course>().HasIndex(i => i.Name).IsUnique(true);
            modelBuilder.Entity<Course>().HasIndex(i => i.EnrollmentKey).IsUnique(true);
            modelBuilder.Entity<DegreeProgram>().HasIndex(i => i.Name).IsUnique(true);
            modelBuilder.Entity<Lecturer>().HasIndex(i => i.Name).IsUnique(true);
            modelBuilder.Entity<Student>().HasIndex(i => i.Name).IsUnique(true);

        }

    }
}
