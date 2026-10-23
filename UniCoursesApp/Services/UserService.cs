using UniCoursesApp.Models;

namespace UniCoursesApp.Services;

public class UserService : IUserService
{
    private readonly List<User> _systemUsers = new()
    {
        new User
        {
            Id = 1,
            UserName = "AdminUser",
            Password = "Admin123",
            Email = "admin@satbayev.kz",
            Role = "Admin"
        },
        new User
        {
            Id = 2,
            UserName = "TeacherUser",
            Password = "Teacher123",
            Email = "teacher@satbayev.kz",
            Role = "Teacher"
        },
        new User
        {
            Id = 3,
            UserName = "StudentUser",
            Password = "Student123",
            Email = "student@satbayev.kz",
            Role = "Student"
        }
    };

    public User? AuthenticateUser(string login, string password)
    {
        Console.WriteLine($"проверяем учетную запись для {login}");

        var account = _systemUsers.FirstOrDefault(u =>
            string.Equals(u.UserName, login, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (account == null)
        {
            Console.WriteLine($"отклонено: неверный логин или пароль для пользователя {login}");
            return null;
        }

        Console.WriteLine($"пользователь {login} успешно авторизован, роль: {account.Role}");
        return account;
    }
}
