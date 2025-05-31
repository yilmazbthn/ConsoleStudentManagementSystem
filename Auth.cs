using System.Security.Cryptography;
using System.Text;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem;

public class Auth
{
    private static readonly AppDbContext _context = new AppDbContext();

    public enum LoginStatus
    {
        LoggedIn,
        UserNotFound,
        Wrong
    }

    public enum RegisterStatus
    {
        Success,
        UsernameExists
    }

    public static RegisterStatus Register(
        string name,
        string lastName,
        string username,
        DateOnly birthday,
        string password,
        UserRoles role,
        out User? loggedInUser,
        string course = "")
    {
        loggedInUser = null;

        if (_context.Users.Any(u => u.Username == username))
            return RegisterStatus.UsernameExists;

        User? newUser = role switch
        {
            UserRoles.Student => new Student
            {
                FirstName = name,
                LastName = lastName,
                Username = username,
                Password = Hash(password),
                Birthday = birthday,
                Classrooms = new List<Classroom>()
            },
            UserRoles.Instructor => new Instructor
            {
                FirstName = name,
                LastName = lastName,
                Username = username,
                Password = Hash(password),
                Birthday = birthday,
                InstructorCourse = course,
                Classrooms = new List<Classroom>()
            },
            UserRoles.Admin => new Admin
            {
                FirstName = name,
                LastName = lastName,
                Username = username,
                Password = Hash(password),
                Birthday = birthday,
            },
            _ => null
        };

        if (newUser == null)
            return RegisterStatus.UsernameExists;

        _context.Users.Add(newUser);
        _context.SaveChanges();

        loggedInUser = newUser;
        return RegisterStatus.Success;
    }

    public static LoginStatus Login(string username, string password, out User? loggedInUser)
    {
        loggedInUser = null;

        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return LoginStatus.UserNotFound;
        }

        if (user.Password != Hash(password))
        {
            return LoginStatus.Wrong;
        }

        loggedInUser = user;
        return LoginStatus.LoggedIn;
    }

    private static string Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        StringBuilder builder = new StringBuilder();

        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
