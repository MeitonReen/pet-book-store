using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Conflict.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.Profile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Create;

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
    /// Create a user profile
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Profile.Build)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] [Required] CreateRequest request)
    {
        var targetResource = _profileResource.Create(request.ToEntity()).ResourceEntity;

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.PostgresUniqueViolation(e))
        {
            return ResultModelBuilder
                .Conflict()
                .ApplyDefaultSettings("Profile already created")
                .Build()
                .ToActionResult();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<CreateResponse>(targetResource);

        return ResultModelBuilder
            .Created(targetResourceToResult)
            .Build()
            .ToActionResult();
    }
}

public static class CreateRequestExtensions
{
    public static BL.ResourceEntities.Profile ToEntity(this CreateRequest request)
        => new()
        {
            UserId = request.UserId,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic
        };
}