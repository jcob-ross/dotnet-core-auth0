namespace dotnet_core_auth0.Backend.Controllers.Apis
{
  using System;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using System.Linq;
  using Infrastructure.Attributes;
  using Models;

  public class SampleApiController : Controller
  {
    [HttpGet]
    [NoCache]
    [Route("api/ping")]
    public IActionResult Ping()
    {
      return Json(new MessageDto
      {
        Message = "Pong. You accessed an unprotected endpoint."
      });
    }

    [HttpGet]
    [NoCache]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/secured-ping")]
    public IActionResult SecuredPing()
    { 
      return Json(new MessageDto
      {
        Message = "Pong. You accessed a protected endpoint.",
      });
    }

    [HttpGet]
    [NoCache]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/user-data")]
    public IActionResult UserData()
    { 
      return Json(new ClaimsDto()
      {
        Claims = User.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
      });
    }

    [HttpGet]
    [NoCache]
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Policy = "OwnerOnly")]
    [Route("api/owner-only")]
    public IActionResult OwnerOnly()
    {
      return Json(new MessageDto
      {
        Message = "Pong. You accessed a protected, owner-only endpoint.",
      });
    }

    [HttpGet]
    [NoCache]
    [Route("api/error")]
    public IActionResult Error()
    {
      throw new Exception("Artificial exception from api/error route.");

      return Json(new MessageDto
      {
        Message = "Exceptional!"
      });
    }
  }
}