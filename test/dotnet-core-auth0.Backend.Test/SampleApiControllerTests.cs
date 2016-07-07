namespace dotnet_core_auth0.Backend.Test
{
  using System;
  using System.Collections.Generic;
  using System.Security.Claims;
  using System.Threading.Tasks;
  using Controllers.Apis;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Http.Features.Authentication;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.DependencyInjection;
  using Models;
  using Xunit;

  public class SampleApiControllerTests
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly SampleApiController _sut;
    public SampleApiControllerTests()
    {
      var services = new ServiceCollection();
      var context = new DefaultHttpContext();
      context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature()
                                                       {
                                                         Handler = new TestAuthHandler()
                                                       });
      services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor()
                {
                  HttpContext = context,
                });
      _serviceProvider = services.BuildServiceProvider();
      _sut = new SampleApiController();
    }

    [Fact]
    public void Ping_should_return_json_result_with_proper_message()
    {
      var result = _sut.Ping();

      var jsonResult = Assert.IsType<JsonResult>(result);
      Assert.Equal((jsonResult.Value as MessageDto).Message, "Pong. You accessed an unprotected endpoint.");
    }

    [Fact]
    public void SecuredPing_should_allow_authenticated_user()
    {
      const string userId = "user-id-1";
      const string userName = "user-name-1";
      var claims = new List<Claim>
                   {
                     new Claim(ClaimTypes.NameIdentifier, userId),
                     new Claim(ClaimTypes.Name, userName)
                   };
      var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
      httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
      _sut.ControllerContext.HttpContext = httpContext;

      // todo: testing - research (how to) - authorization, status codes, headers etc.
      throw new NotImplementedException();

      var result = _sut.SecuredPing();

      var jsonResult = Assert.IsType<JsonResult>(result);
      Assert.Equal((jsonResult.Value as MessageDto).Message, "Pong. You accessed a protected endpoint.");
    }

    [Fact]
    public void SecuredPing_should_disallow_unauthenticated_user()
    {
      const string userId = "user-id-1";
      const string userName = "user-name-1";
      var claims = new List<Claim>
                   {
                     new Claim(ClaimTypes.NameIdentifier, userId),
                     new Claim(ClaimTypes.Name, userName)
                   };
      var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
      httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
      _sut.ControllerContext.HttpContext = httpContext;

      // todo: testing - research (how to) - authorization, status codes, headers etc.
      throw new NotImplementedException();

      var result = _sut.SecuredPing();

      var jsonResult = Assert.IsType<JsonResult>(result);
    }

    private class TestAuthHandler : IAuthenticationHandler
    {
      public void Authenticate(AuthenticateContext context)
      {
        context.NotAuthenticated();
      }

      public Task AuthenticateAsync(AuthenticateContext context)
      {
        context.NotAuthenticated();
        return Task.FromResult(0);
      }

      public Task ChallengeAsync(ChallengeContext context)
      {
        throw new NotImplementedException();
      }

      public void GetDescriptions(DescribeSchemesContext context)
      {
        throw new NotImplementedException();
      }

      public Task SignInAsync(SignInContext context)
      {
        throw new NotImplementedException();
      }

      public Task SignOutAsync(SignOutContext context)
      {
        throw new NotImplementedException();
      }
    }
  }
}