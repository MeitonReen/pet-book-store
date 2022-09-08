using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Exception.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Update;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly AppConfig _appConfig;
    private readonly IRequestClient<UpdateBookRequest> _bookUpdateRequest;
    private readonly IMapper _mapper;

    public Controller(
        IMapper mapper,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        IRequestClient<UpdateBookRequest> bookUpdateRequest,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _appConfig = appConfigAccessor.Value;
        _bookUpdateRequest = bookUpdateRequest;
        _mapper = mapper;
    }

    /// <summary>
    /// Update a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully updated</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{U}"
               + Or +
               $"{DefaultBookStoreResources}{U}")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Book.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] [Required] UpdateRequest request)
    {
        var result = await _bookUpdateRequest.GetResponse<Success, Failed>(request);

        if (result.Is<Success>(out _))
        {
            return ResultModelBuilder
                .Updated(_mapper.Map<UpdateResponse>(request))
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