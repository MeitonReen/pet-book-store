using BookStore.AuthorizationService.BL.ResourceEntities;
using BookStore.AuthorizationService.Contracts.Connect.V1_0_0;
using BookStore.AuthorizationService.WebEntryPoint.Connect.V1_0_0.Helpers;
using BookStore.AuthorizationService.WebEntryPoint.Helpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.ResourcesAuthorization.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace BookStore.AuthorizationService.WebEntryPoint.Connect.V1_0_0.CreateAuthorization;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public Controller(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ILogger<Controller>? logger = default
    )
        : base(logger)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [Route(HttpApiRouteBuilder.Connect.Authorization.Build)]
    [HttpGet]
    public async Task<IActionResult> CreateAuthorization()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        ;

        if (!request.IsAuthorizationCodeFlow()) return Unauthorized();

        var targetUser = await _userManager.FindByNameAsync(request.Username);
        if (targetUser == default) return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(targetUser, request.Password)) return Unauthorized();

        if (request.ClientId == default) return Unauthorized();
        var relyingParty = await _applicationManager.FindByClientIdAsync(request.ClientId);
        if (relyingParty == default) return Unauthorized();

        var targetUserPrincipal = await _signInManager.CreateUserPrincipalAsync(targetUser);
        if (targetUserPrincipal == default) return Unauthorized();

        var userScopes = targetUserPrincipal.GetScopes();
        var requestedScopes = request.GetScopes();

        if (!requestedScopes.All(requestedScope =>
                userScopes.Any(userScope =>
                    ScopeHelper.FirstScopeIncludesSecondScope(userScope, requestedScope))))
            return new StatusCodeResult(StatusCodes.Status403Forbidden);

        if (request.HasPrompt(OpenIddictConstants.Prompts.Consent)) return Ok();

        //requestedScopes == consented scopes
        targetUserPrincipal.SetScopes(requestedScopes);
        ;
        var pastAuthorizations = await _authorizationManager.FindAsync(
            subject: await _userManager.GetUserIdAsync(targetUser),
            client: await _applicationManager.GetIdAsync(relyingParty),
            status: OpenIddictConstants.Statuses.Valid,
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes: request.GetScopes()).ToListAsync();
        ;

        var targetAuthorization = pastAuthorizations.LastOrDefault() ?? await _authorizationManager.CreateAsync(
            principal: targetUserPrincipal,
            subject: await _userManager.GetUserIdAsync(targetUser),
            client: await _applicationManager.GetIdAsync(relyingParty),
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes: targetUserPrincipal.GetScopes());

        targetUserPrincipal.SetAuthorizationId(await _authorizationManager.GetIdAsync(targetAuthorization));

        foreach (var claim in targetUserPrincipal.Claims)
        {
            claim.SetDestinations(ClaimDestinations.Get(claim, targetUserPrincipal));
        }

        return SignIn(targetUserPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}