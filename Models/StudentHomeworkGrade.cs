namespace StudentManagementSystem.Models;

public class StudentHomeworkGrade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int HomeworkId { get; set; }
    public Homework Homework { get; set; }
    public int? Grade { get; set; }
    public string? Feedback { get; set; }
}