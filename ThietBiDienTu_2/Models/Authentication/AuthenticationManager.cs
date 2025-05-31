using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ThietBiDienTu_2.Models.Authentication
{
    public class AuthenticationManager : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetInt32("TypeAccount") == null || context.HttpContext.Session.GetInt32("TypeAccount") != 2)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller","Access"},
                        {"Action","Login"},
                        {"Area","" }
                    });
            }

        }
    }
}
