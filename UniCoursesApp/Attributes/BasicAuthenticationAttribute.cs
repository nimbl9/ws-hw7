using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniCoursesApp.Services;

namespace UniCoursesApp.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthenticationAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
{
    public int Order => 1;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            Console.WriteLine("ошибка: отсутствует заголовок authorization в запросе к курсам");
            context.Result = new UnauthorizedResult();
            return;
        }

        string authHeader = authHeaderValue.ToString();
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("ошибка: схема аутентификации должна быть basic");
            context.Result = new UnauthorizedResult();
            return;
        }

        string base64Encoded = authHeader.Substring("Basic ".Length).Trim();
        string decodedText;

        try
        {
            byte[] rawBytes = Convert.FromBase64String(base64Encoded);
            decodedText = Encoding.UTF8.GetString(rawBytes);
        }
        catch (FormatException)
        {
            Console.WriteLine("ошибка: передан некорректный base64 токен");
            context.Result = new UnauthorizedResult();
            return;
        }

        int colonIndex = decodedText.IndexOf(':');
        if (colonIndex <= 0)
        {
            Console.WriteLine("ошибка: разделитель логина и пароля не найден");
            context.Result = new UnauthorizedResult();
            return;
        }

        string username = decodedText.Substring(0, colonIndex);
        string password = decodedText.Substring(colonIndex + 1);

        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var user = userService.AuthenticateUser(username, password);

        if (user == null)
        {
            Console.WriteLine($"отклонено: учетные данные пользователя {username} не прошли валидацию");
            context.Result = new UnauthorizedResult();
            return;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        Console.WriteLine($"пользователь {user.UserName} с ролью {user.Role} успешно аутентифицирован");
    }
}
