using UniCoursesApp.Models;

namespace UniCoursesApp.Services;

public interface IUserService
{
    User? AuthenticateUser(string login, string password);
}
