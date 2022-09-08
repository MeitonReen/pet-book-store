using System.Security.Claims;
using BookStore.AuthorizationService.BL.ResourceEntities;
using BookStore.AuthorizationService.Contracts.Account.V1_0_0;
using BookStore.AuthorizationService.Contracts.Account.V1_0_0.Create;
using BookStore.AuthorizationService.Defaults;
using BookStore.Base.Implementations.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace BookStore.AuthorizationService.WebEntryPoint.Account.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly UserManager<User> _userManager;

    public Controller(UserManager<User> userManager,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _userManager = userManager;
    }

    [Route(HttpApiRouteBuilder.Account.Build)]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);
        if (user != default) return StatusCode(StatusCodes.Status409Conflict);

        user = new User {UserName = request.Login};
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest("Registration error");

        result = await _userManager.AddClaimAsync(user, new Claim(OpenIddictConstants.Claims.Private.Scope,
            BookStoreDefaultScopes.DefaultUserResourcesCRUD));

        if (result.Succeeded) return Ok();

        await _userManager.DeleteAsync(user);
        return BadRequest("Registration error");
    }
}