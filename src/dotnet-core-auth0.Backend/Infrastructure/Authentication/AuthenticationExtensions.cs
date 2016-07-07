namespace dotnet_core_auth0.Backend.Infrastructure.Authentication
{
  using System;
  using System.Security.Claims;
  using System.Security.Cryptography.X509Certificates;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using Microsoft.IdentityModel.Tokens;

  public class Auth0Settings
  {
    public string Domain { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CertificateString { get; set; }
  }

  public static class AuthenticationExtensions
  {
    private static ILogger _logger;

    public static void UseAuth0JwtAuthentication(this IApplicationBuilder app, string protocolScheme = "https")
    {
      if (null == app)
        throw new ArgumentNullException(nameof(app));
      if (protocolScheme != "http" && protocolScheme != "https")
        throw new ArgumentException("Invalid protocol scheme. Only http and https are allowed");

      _logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Auth0-Jwt-Authentication");
      var settings = app.ApplicationServices.GetService<IOptions<Auth0Settings>>().Value;

      var certificateString = settings.CertificateString;
      var certificate = new X509Certificate2(Convert.FromBase64String(certificateString));

      var jwtOptions = new JwtBearerOptions();
      jwtOptions.Audience = settings.ClientId;
      jwtOptions.Authority = $"{protocolScheme}://{settings.Domain}";
      jwtOptions.AutomaticAuthenticate = true;
      jwtOptions.AutomaticChallenge = true;
      jwtOptions.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(15);
      jwtOptions.TokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);

      if (protocolScheme == "http")
        _logger.LogWarning("Using http scheme");

      jwtOptions.RequireHttpsMetadata = protocolScheme != "http";

      jwtOptions.Events = CreateJwtEvents();
      app.UseJwtBearerAuthentication(jwtOptions);
    }

    private static JwtBearerEvents CreateJwtEvents()
    {
      var jwtEvents = new JwtBearerEvents();
      jwtEvents.OnAuthenticationFailed = (ctx) =>
      {
        _logger.LogError("Authentication failed.", ctx.Exception);
        return Task.FromResult(0);
      };

      jwtEvents.OnTokenValidated = (ctx) =>
      {
        // var claimsIdentity = ctx.Ticket.Principal.Identity as ClaimsIdentity;
        // ...
        // claimsIdentity.AddClaim(claim);
        // use this and/or AuthClaimsTransformer
        return Task.FromResult(0);
      };

      return jwtEvents;
    }
  }
}