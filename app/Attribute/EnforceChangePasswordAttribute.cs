using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Models;

namespace MvcMovie.Attribute
{
    public class EnforceChangePasswordAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userManager  = filterContext.HttpContext.RequestServices.GetService<UserManager<AppUser>>();
            var user =  userManager?.GetUserAsync(filterContext.HttpContext.User).GetAwaiter().GetResult();
            if (user is {EnforceChangePassword: true})
                filterContext.Result = new RedirectResult("/Account/SetPassword");
        }
    }
}
