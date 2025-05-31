using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<ClassroomInstructor> ClassroomInstructors { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<StudentHomeworkGrade> StudentHomeworkGrades { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost;Database=StudentManagementSystem;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasDiscriminator<UserRoles>("Role")
            .HasValue<Admin>(UserRoles.Admin)
            .HasValue<Student>(UserRoles.Student)
            .HasValue<Instructor>(UserRoles.Instructor);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.Classrooms)
            .WithMany(c => c.Students);

        modelBuilder.Entity<Instructor>()
            .HasMany(i => i.Classrooms)
            .WithMany(c => c.Instructors);


        modelBuilder.Entity<Lesson>()
            .HasMany(l => l.Classrooms)
            .WithMany(c => c.Lessons);


        modelBuilder.Entity<Lesson>()
            .HasMany(l => l.Instructors)
            .WithMany(i => i.Lessons);

   
        modelBuilder.Entity<StudentHomeworkGrade>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId);

        modelBuilder.Entity<StudentHomeworkGrade>()
            .HasOne(x => x.Homework)
            .WithMany()
            .HasForeignKey(x => x.HomeworkId);
        modelBuilder.Entity<ClassroomInstructor>()
            .HasKey(ci => new { ci.ClassroomId, ci.InstructorId });

        base.OnModelCreating(modelBuilder);
    }
   


}