namespace dotnet_core_auth0.Backend.Models.Authorization
{
  using System.Collections.Generic;
  using Microsoft.AspNetCore.Mvc.ModelBinding;

  public class LogoutViewModel
  {
    [BindNever]
    public IDictionary<string, string> Parameters { get; set; }
  }
}