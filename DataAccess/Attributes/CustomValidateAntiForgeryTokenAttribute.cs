using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Utility;

public class CustomValidateAntiForgeryTokenAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();
        if (antiforgery == null)
        {
            context.Result = new BadRequestObjectResult("Không thể xác thực CSRF token");
            return;
        }

        if (context.HttpContext.Request.Headers.TryGetValue("X-APP-SOURCE", out var appSourceValues))
        {
            // Kiểm tra và so sánh giá trị của header X-App-Source
            if (appSourceValues.Equals(SD.XAPPSOURCE))
            {
                return;
            }
        }

        try
        {
            // Thực hiện validate CSRF token
            var task = antiforgery.ValidateRequestAsync(context.HttpContext);
            task.GetAwaiter().GetResult();
        }
        catch (AntiforgeryValidationException ex)
        {
            context.Result = new BadRequestObjectResult(ex.Message);
            return;
        }
        catch (SecurityTokenException ex)
        {
            context.Result = new BadRequestObjectResult(ex.Message);
            return;
        }
        catch (Exception ex)
        {
            context.Result = new BadRequestObjectResult("Lỗi xác thực");
            return;
        }
    }
}
