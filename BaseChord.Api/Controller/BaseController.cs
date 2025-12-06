using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseChord.Api.Controller;

/// <summary>
/// Represents a base controller class that provides shared functionality for derived API controllers.
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the instance of <see cref="ISender"/> used to send commands or queries
    /// within the MediatR library for handling application logic.
    /// </summary>
    /// <remarks>
    /// This property is initialized through the constructor of the <see cref="BaseController"/>
    /// and cannot be modified after initialization. It is intended to provide a unified
    /// way of interacting with the MediatR pipeline in derived controllers.
    /// </remarks>
    protected ISender Sender { get; init; }

    /// <summary>
    /// Represents a base controller class that provides shared functionality for derived API controllers.
    /// </summary>
    protected BaseController(ISender sender) => Sender = sender;

    /// <summary>
    /// Searches the httpcontext for the external identifier provided by auth0
    /// </summary>
    /// <returns>The external identifier provided via an JWT</returns>
    public string? GetExternalIdentifier()
    {
        return (HttpContext.User.Identity as ClaimsIdentity)?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }
}
