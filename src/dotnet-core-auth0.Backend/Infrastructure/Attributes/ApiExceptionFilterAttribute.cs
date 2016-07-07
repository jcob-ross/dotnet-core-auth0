namespace dotnet_core_auth0.Backend.Infrastructure.Attributes
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Filters;
  using System.Linq;

  /// <summary>
  ///   Filters exceptions for requests with application/json media type header
  ///   and creates json result containing message property with exception message
  /// </summary>
  public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
  {
    public override void OnException(ExceptionContext context)
    {
      CreateResult(context);
    }

    public override Task OnExceptionAsync(ExceptionContext context)
    {
      CreateResult(context);
      return Task.FromResult(0);
    }

    private void CreateResult(ExceptionContext context)
    {
      if (context.HttpContext.Request.GetTypedHeaders().Accept.Any(header => header.MediaType == "application/json"))
      {
        var jsonResult = new JsonResult(new { message = context.Exception.Message });
        jsonResult.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        context.Result = jsonResult;
      }
    }
  }
}