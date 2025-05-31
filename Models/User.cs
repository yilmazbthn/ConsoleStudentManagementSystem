using StudentManagementSystem.Models;

public abstract class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual UserRoles Role { get; set; }

}