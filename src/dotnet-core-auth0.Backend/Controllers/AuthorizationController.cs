﻿namespace dotnet_core_auth0.Backend.Controllers
{
  using System;
  using System.Security.Claims;
  using System.Threading;
  using System.Threading.Tasks;
  using AspNet.Security.OpenIdConnect.Extensions;
  using AspNet.Security.OpenIdConnect.Server;
  using Data.Models;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Http.Authentication;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;
  using Models.Authorization;
  using OpenIddict;

  public class AuthorizationController : Controller
  {
    private readonly OpenIddictApplicationManager<OpenIddictApplication<Guid>> _applicationManager;
    private readonly SignInManager<SpaUser> _signInManager;
    private readonly OpenIddictUserManager<SpaUser> _userManager;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
            OpenIddictApplicationManager<OpenIddictApplication<Guid>> applicationManager,
            SignInManager<SpaUser> signInManager,
            OpenIddictUserManager<SpaUser> userManager,
            ILogger<AuthorizationController> logger)
    {
      _applicationManager = applicationManager;
      _signInManager = signInManager;
      _userManager = userManager;
      _logger = logger;
    }

    [Authorize, HttpGet, Route("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
      // Extract the authorization request from the ASP.NET environment.
      var request = HttpContext.GetOpenIdConnectRequest();

      // Retrieve the application details from the database.
      var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
      if (application == null)
      {
        return View("Error", new ErrorViewModel
        {
          Error = OpenIdConnectConstants.Errors.InvalidClient,
          ErrorDescription = "Details concerning the calling client application cannot be found in the database"
        });
      }

      return View(new AuthorizeViewModel
      {
        ApplicationName = application.DisplayName,
        Parameters = request.Parameters,
        Scope = request.Scope
      });
    }

    [Authorize, HttpPost("~/connect/authorize/accept"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept()
    {
      // Extract the authorization request from the ASP.NET environment.
      var request = HttpContext.GetOpenIdConnectRequest();

      // Retrieve the profile of the logged in user.
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return View("Error", new ErrorViewModel
        {
          Error = OpenIdConnectConstants.Errors.ServerError,
          ErrorDescription = "An internal error has occurred"
        });
      }

      // Create a new ClaimsIdentity containing the claims that
      // will be used to create an id_token, a token or a code.
      var identity = await _userManager.CreateIdentityAsync(user, request.GetScopes());

      // Create a new authentication ticket holding the user identity.
      var ticket = new AuthenticationTicket(
          new ClaimsPrincipal(identity),
          new AuthenticationProperties(),
          OpenIdConnectServerDefaults.AuthenticationScheme);

      ticket.SetResources(request.GetResources());
      ticket.SetScopes(request.GetScopes());

      // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
      return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
    }

    [Authorize, HttpPost("~/connect/authorize/deny"), ValidateAntiForgeryToken]
    public IActionResult Deny()
    {
      // Notify OpenIddict that the authorization grant has been denied by the resource owner
      // to redirect the user agent to the client application using the appropriate response_mode.
      return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
    }

    [HttpGet("~/connect/logout")]
    public IActionResult Logout()
    {
      // Extract the authorization request from the ASP.NET environment.
      var request = HttpContext.GetOpenIdConnectRequest();

      return View(new LogoutViewModel
      {
        Parameters = request.Parameters
      });
    }

    [HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
      // Ask ASP.NET Core Identity to delete the local and external cookies created
      // when the user agent is redirected from the external identity provider
      // after a successful authentication flow (e.g Google or Facebook).
      await _signInManager.SignOutAsync();

      // Returning a SignOutResult will ask OpenIddict to redirect the user agent
      // to the post_logout_redirect_uri specified by the client application.
      return SignOut(OpenIdConnectServerDefaults.AuthenticationScheme);
    }

    [HttpGet, HttpPost, Route("~/connect/error")]
    public IActionResult Error()
    {
      var response = HttpContext.GetOpenIdConnectResponse();
      if (response == null)
      {
        return View(new ErrorViewModel
        {
          Error = OpenIdConnectConstants.Errors.InvalidRequest
        });
      }

      return View(new ErrorViewModel
      {
        Error = response.Error,
        ErrorDescription = response.ErrorDescription
      });
    }
  }
}