using AutoMapper;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Exception.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.DeleteOut;
using BookStore.UserService.Contracts.MyProfile.V1_0_0;
using BookStore.UserService.Contracts.Profile.V1_0_0.Delete;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyProfile.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly AppConfig _appConfig;
    private readonly IRequestClient<DeleteProfileRequest> _deleteProfileRequest;
    private readonly IMapper _mapper;

    private readonly IUserClaimsProfile _userClaimsProfile;

    public Controller(
        IMapper mapper,
        IUserClaimsProfile userClaimsProfile,
        IRequestClient<DeleteProfileRequest> deleteProfileRequest,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _appConfig = appConfigAccessor.Value;
        _mapper = mapper;
        _userClaimsProfile = userClaimsProfile;
        _deleteProfileRequest = deleteProfileRequest;
    }

    /// <summary>
    /// Delete my profile
    /// </summary>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{D}"
               + Or +
               $"{DefaultBookStoreResources}{D}"
               + Or +
               $"{DefaultUserResources}{D}")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyProfile.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var request = new DeleteRequest {UserId = Guid.Parse(_userClaimsProfile.UserId)};

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