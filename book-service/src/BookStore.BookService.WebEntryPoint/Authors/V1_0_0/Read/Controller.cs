using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.Author.V1_0_0.Read;
using BookStore.BookService.Contracts.Authors.V1_0_0;
using BookStore.BookService.Contracts.Authors.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.BookService.WebEntryPoint.Authors.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.Author>
        _authorResourceCollection;

    private readonly IBaseResourceCollectionCount<BL.ResourceEntities.Author>
        _authorResourceCollectionCount;

    private readonly IConfigurationProvider _mapperConfigurationProvider;
    // private readonly ICacheInvalidator _cacheInvalidator;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.Author> authorResourceCollection,
        IBaseResourceCollectionCount<BL.ResourceEntities.Author> authorResourceCollectionCount,
        IConfigurationProvider mapperConfigurationProvider,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _authorResourceCollection = authorResourceCollection;
        _authorResourceCollectionCount = authorResourceCollectionCount;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Read authors
    /// </summary>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Authors.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _authorResourceCollection
            .ReadSettings(sets => sets
                .OrderBy(author => author.AuthorId)
                .ReadPart(request)
                .ProjectTo<ReadResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        IEnumerable<ReadResponse> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }

        var targetResult = targetResource
            .ToDataPartResponse(request, await _authorResourceCollectionCount.ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}