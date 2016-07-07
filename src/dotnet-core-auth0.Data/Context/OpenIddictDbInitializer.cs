namespace dotnet_core_auth0.Data.Context
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Identity;
  using Models;
  using OpenIddict;

  public class OpenIddictDbInitializer : IOpenIddictDbInitializer
  {
    private readonly SpaDbContext _dbContext;
    private readonly UserManager<SpaUser> _userManager;

    public OpenIddictDbInitializer(SpaDbContext dbContext, UserManager<SpaUser> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    public async Task InitializeAsync()
    {
      await _dbContext.Database.EnsureCreatedAsync();
      await _userManager.CreateAsync(new SpaUser
      {
        Email = "admin@it.io",
        UserName = "admin@it.io",
        EmailConfirmed = true,
      }, "password");

      // Note: when using the introspection middleware, your resource server
      // MUST be registered as an OAuth2 client and have valid credentials.
      // 
      // context.Applications.Add(new OpenIddictApplication {
      //     Id = "resource_server",
      //     DisplayName = "Main resource server",
      //     Secret = Crypto.HashPassword("secret_secret_secret"),
      //     Type = OpenIddictConstants.ClientTypes.Confidential
      // });

      var spaApp = new OpenIddictApplication<Guid>
                   {
                     Id = Guid.NewGuid(),
                     ClientId = "angular 2 client",
                     DisplayName = "Angular 2 Client App",
                     LogoutRedirectUri = "http://localhost:8080/",
                     RedirectUri = "http://localhost:8080/signin-oidc",
                     Type = OpenIddictConstants.ClientTypes.Public
                   };

      // To test this sample with Postman, use the following settings:
      // 
      // * Authorization URL: http://localhost:8080/connect/authorize
      // * Access token URL: http://localhost:8080/connect/token
      // * Client ID: postman
      // * Client secret: [blank] (not used with public clients)
      // * Scope: openid email profile roles
      // * Grant type: authorization code
      // * Request access token locally: yes

      var postmanApp = new OpenIddictApplication<Guid>
                       {
                         Id = Guid.NewGuid(),
                         ClientId = "postman",
                         DisplayName = "Postman",
                         RedirectUri = "https://www.getpostman.com/oauth2/callback",
                         Type = OpenIddictConstants.ClientTypes.Public
                       };
      _dbContext.Applications.Add(spaApp);
      _dbContext.Applications.Add(postmanApp);

      await _dbContext.SaveChangesAsync();
    }
  }
}