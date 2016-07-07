namespace dotnet_core_auth0.Backend.Models
{
  using System.Collections.Generic;

  public class ClaimsDto
  {
    public IList<ClaimDto> Claims { get; set; } = new List<ClaimDto>();
  }
}