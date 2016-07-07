namespace dotnet_core_auth0.Backend.Models.Authorization
{
  using System.ComponentModel.DataAnnotations;

  public class ErrorViewModel
  {
    [Display(Name = "Error")]
    public string Error { get; set; }

    [Display(Name = "Description")]
    public string ErrorDescription { get; set; }
  }
}