using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Conflict.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.MyProfile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;
using CreateRequest = BookStore.UserService.Contracts.MyProfile.V1_0_0.Create.CreateRequest;

namespace BookStore.UserService.WebEntryPoint.MyProfile.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    private readonly IUserClaimsProfile _userClaimsProfile;

    public Controller(
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        IUserClaimsProfile userClaimsProfile,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _profileResource = profileResource;
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
        _userClaimsProfile = userClaimsProfile;
    }

    /// <summary>
    /// Create my profile
    /// </summary>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{C}"
               + Or +
               $"{DefaultBookStoreResources}{C}"
               + Or +
               $"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyProfile.Build)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] [Required] CreateRequest request)
    {
        var userProfile = request.ToEntity();
        userProfile = _userClaimsProfile.FillEntity(userProfile);

        userProfile = _profileResource.Create(userProfile).ResourceEntity;

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

        var targetResourceToResult = _mapper.Map<CreateResponse>(userProfile);

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
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic
        };
}

public static class UserClaimsProfileExtensions
{
    public static BL.ResourceEntities.Profile FillEntity(this IUserClaimsProfile userClaimsProfile,
        BL.ResourceEntities.Profile entity)
    {
        entity.UserId = Guid.Parse(userClaimsProfile.UserId);
        entity.UserName = userClaimsProfile.Name;

        return entity;
    }
}