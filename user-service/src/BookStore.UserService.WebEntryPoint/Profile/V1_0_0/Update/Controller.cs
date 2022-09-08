using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.Profile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Update;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        ILogger<Controller>? logger = default)
        : base(logger)
    {
        _profileResource = profileResource;
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
    }

    /// <summary>
    /// Update a user profile
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully updated</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{U}")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Profile.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update(
        [FromBody] [Required] UpdateRequest request)
    {
        var targetResource = _profileResource.Update(request.ToEntity()).ResourceEntity;

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<UpdateResponse>(targetResource);

        return ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToActionResult();
    }
}

public static class UpdateRequestExtensions
{
    public static BL.ResourceEntities.Profile ToEntity(this UpdateRequest request)
        => new()
        {
            UserId = request.UserId,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic
        };
}