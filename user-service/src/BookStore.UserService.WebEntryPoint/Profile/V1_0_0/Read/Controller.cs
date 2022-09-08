using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.NotFound.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.Profile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Read;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;

    public Controller(
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        IPublishEndpoint transactionOutboxPublishEndpoint,
        ILogger<Controller>? logger = default)
        : base(logger)
    {
        _profileResource = profileResource;
        _mapper = mapper;
    }

    /// <summary>
    /// Read a user profile
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{R}")]
    [ProducesResponseType(typeof(ReadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Profile.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadRequest request)
    {
        var configuredResource = _profileResource
            .ReadSettings(settings => settings
                .Where(profile => profile.UserId == request.UserId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        return targetResult != default
            ? ResultModelBuilder
                .Read(targetResult)
                .Build()
                .ToActionResult()
            : ResultModelBuilder
                .NotFound()
                .ApplyDefaultSettings()
                .Build()
                .ToActionResult();
    }
}