namespace dotnet_core_auth0.Data.Context
{
  using System;
  using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore;
  using Models;
  using OpenIddict;

  public class SpaDbContext : OpenIddictDbContext<SpaUser, IdentityRole<Guid>, Guid>
  {
    public SpaDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}