using AutoMapper;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Exception.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.DeleteOut;
using BookStore.UserService.Contracts.Profile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Delete;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly AppConfig _appConfig;
    private readonly IRequestClient<DeleteProfileRequest> _deleteProfileRequest;
    private readonly IMapper _mapper;

    public Controller(
        IMapper mapper,
        IRequestClient<DeleteProfileRequest> deleteProfileRequest,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        ILogger<Controller>? logger = default
    )
        : base(logger)
    {
        _appConfig = appConfigAccessor.Value;
        _mapper = mapper;
        _deleteProfileRequest = deleteProfileRequest;
    }

    /// <summary>
    /// Delete a user profile
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{D}")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Profile.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteRequest request)
    {
        var result = await _deleteProfileRequest.GetResponse<Success, Failed>(request);

        if (result.Is<Success>(out _))
        {
            return ResultModelBuilder
                .Deleted(_mapper.Map<DeleteResponse>(request))
                .Build()
                .ToResultModel()
                .ToActionResult();
        }

        return ResultModelBuilder
            .Exception()
            .ApplyDefaultSettings()
            .Environment(_appConfig.Environment)
            .Build()
            .ToResultModel()
            .ToActionResult();
    }
}