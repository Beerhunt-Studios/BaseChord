using System;
using Microsoft.AspNetCore.Authorization;

namespace BaseChord.Api.Middleware.Authorization;

/// <summary>
/// Custom authorization requirement that checks if the user has a specific scope
/// </summary>
public class HasScopeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The issuer of the scope claim
    /// </summary>
    public string Issuer { get; }
    
    /// <summary>
    /// The scope claim
    /// </summary>
    public string Scope { get; }

    /// <summary>
    /// Constructor of the HasScopeRequirement
    /// </summary>
    /// <param name="scope">The scope claim that is required</param>
    /// <param name="issuer">The issuer of the scope claim</param>
    /// <exception cref="ArgumentNullException">Thrown when scope or issuer is null</exception>
    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}
