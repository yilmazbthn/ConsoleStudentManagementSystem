using StudentManagementSystem.Models;

public class Lesson
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Schedule { get; set; }
    public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
    public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}