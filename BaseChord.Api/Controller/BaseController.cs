using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseChord.Api.Controller;

public abstract class BaseController : ControllerBase
{
    protected ISender Sender { get; init; }

    public BaseController(ISender sender) => Sender = sender;

    /// <summary>
    /// Searches the httpcontext for the external identifier provided by auth0
    /// </summary>
    /// <returns>The external identifier provided via an JWT</returns>
    public string? GetExternalIdentifier()
    {
        return (HttpContext.User.Identity as ClaimsIdentity)?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }
}
