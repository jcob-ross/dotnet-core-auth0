namespace dotnet_core_auth0.Backend.Models.Authorization
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using Microsoft.AspNetCore.Mvc.ModelBinding;

  public class AuthorizeViewModel
  {
    [Display(Name = "Application")]
    public string ApplicationName { get; set; }

    [BindNever]
    public IDictionary<string, string> Parameters { get; set; }

    [Display(Name = "Scope")]
    public string Scope { get; set; }
  }
}