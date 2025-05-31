namespace StudentManagementSystem.Models;

public class ClassroomInstructor
{
    public int ClassroomId { get; set; }
    public Classroom Classroom { get; set; }

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }
}