namespace dotnet_core_auth0.Backend.Infrastructure
{
  using System.Security.Claims;
  using Microsoft.AspNetCore.Authentication;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Logging;

  public class AuthClaimsTransformer : IClaimsTransformer
  {
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuthClaimsTransformer> _logger;
    private bool _done; // see below

    public AuthClaimsTransformer(IMemoryCache cache, ILogger<AuthClaimsTransformer> logger)
    {
      // inject db context / repo, add stored claims
      _cache = cache;
      _logger = logger;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsTransformationContext context)
    {
      _logger.LogDebug("Running claims transformation");

      // Workaround, transformation runs twice (before and after JwtBearerMiddleware)
      // Issue is on backlog as of July 6th.
      // https://github.com/aspnet/Security/issues/284
      if (_done)
        return Task.FromResult(context.Principal);

      if (context.Principal.Identity.IsAuthenticated)
      {
        // get this from cache or db
        var country = "CZE";
        (context.Principal.Identity as ClaimsIdentity).AddClaim(new Claim("Nationality", country));
        _done = true;
      }
      return Task.FromResult(context.Principal);
    }
  }
}