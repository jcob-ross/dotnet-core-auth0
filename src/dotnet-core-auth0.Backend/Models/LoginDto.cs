namespace dotnet_core_auth0.Backend.Models
{
  using System.ComponentModel.DataAnnotations;

  public class LoginDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

  }
}