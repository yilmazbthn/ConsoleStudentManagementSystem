using StudentManagementSystem.Models;


public class Classroom
{
    public int Id { get; set; }
    public string ClassroomName { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
