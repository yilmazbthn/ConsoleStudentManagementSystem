using StudentManagementSystem.Models;

namespace StudentManagementSystem.Models;

public class Admin : User
{
    public override UserRoles Role => UserRoles.Admin;
    public string Email { get; set; } = string.Empty;
}