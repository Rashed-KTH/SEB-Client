using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AccountInformation.Client.Filters
{
    public class HeaderActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var accessToken = context.HttpContext.Request.Cookies["access_token"];

            if (accessToken != null)
            {
                context.HttpContext.Request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            base.OnActionExecuting(context);
        }
    }
}
