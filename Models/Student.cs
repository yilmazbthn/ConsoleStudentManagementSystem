using StudentManagementSystem.Models;

public class Student : User
{
    public override UserRoles Role => UserRoles.Student;
    public int StudentNumber { get; set; }
    public List<int> EnrolledLessonIds { get; set; } = new();
    public List<Classroom> Classrooms { get; set; } = new();
    public List<Homework> Homeworks { get; set; } = new();
}