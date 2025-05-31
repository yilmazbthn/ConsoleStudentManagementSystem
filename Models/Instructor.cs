namespace StudentManagementSystem.Models;

public class Instructor : User
{

    public override UserRoles Role => UserRoles.Instructor;
    public string InstructorCourse { get; set; }
    public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}