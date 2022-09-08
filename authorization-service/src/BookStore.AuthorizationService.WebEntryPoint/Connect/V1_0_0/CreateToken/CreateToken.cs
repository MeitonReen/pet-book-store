using BookStore.AuthorizationService.BL.ResourceEntities;
using BookStore.AuthorizationService.Contracts.Connect.V1_0_0;
using BookStore.AuthorizationService.WebEntryPoint.Connect.V1_0_0.Helpers;
using BookStore.Base.Implementations.Controller;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace BookStore.AuthorizationService.WebEntryPoint.Connect.V1_0_0.CreateToken;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public Controller(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ILogger<Controller>? logger = default
    )
        : base(logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [Route(HttpApiRouteBuilder.Connect.Token.Build)]
    [HttpPost, Produces("application/json")]
    public async Task<IActionResult> CreateToken()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == default) return BadRequest();

        if (!(request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType()))
            return BadRequest();
        // get principal from auth code
        var targetUserPrincipal =
            (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

        var user = await _userManager.GetUserAsync(targetUserPrincipal);
        if (user == default) return Unauthorized();

        // Ensure the user is still allowed to sign in.
        if (!await _signInManager.CanSignInAsync(user)) return Unauthorized();

        foreach (var claim in targetUserPrincipal.Claims)
        {
            claim.SetDestinations(ClaimDestinations.Get(claim, targetUserPrincipal));
        }

        return SignIn(targetUserPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}