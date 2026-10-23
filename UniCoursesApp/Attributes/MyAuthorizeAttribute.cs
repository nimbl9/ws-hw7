using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniCoursesApp.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
{
    private readonly string[] _permittedRoles;

    public int Order => 2;

    public MyAuthorizeAttribute(params string[] permittedRoles)
    {
        _permittedRoles = permittedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return;
        }

        if (context.Result != null)
        {
            return;
        }

        var currentUser = context.HttpContext.User;
        if (currentUser?.Identity == null || !currentUser.Identity.IsAuthenticated)
        {
            Console.WriteLine("доступ заблокирован: требуется базовая аутентификация пользователя");
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_permittedRoles.Length > 0)
        {
            bool hasAccess = false;
            foreach (var role in _permittedRoles)
            {
                if (currentUser.IsInRole(role))
                {
                    hasAccess = true;
                    break;
                }
            }

            if (!hasAccess)
            {
                Console.WriteLine($"доступ запрещен: пользователю {currentUser.Identity.Name} не разрешено это действие");
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
        }

        Console.WriteLine($"ролевой доступ подтвержден для пользователя {currentUser.Identity.Name}");
    }
}
