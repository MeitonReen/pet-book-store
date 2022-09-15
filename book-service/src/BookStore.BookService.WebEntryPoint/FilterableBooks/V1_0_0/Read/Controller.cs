using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.NonAttachedExtensions;
using BookStore.BookService.Contracts.Book.V1_0_0.Read;
using BookStore.BookService.Contracts.FilterableBooks.V1_0_0;
using BookStore.BookService.Contracts.FilterableBooks.V1_0_0.Read;
using BookStore.BookService.WebEntryPoint.FilterableBooks.V1_0_0.Read.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.BookService.WebEntryPoint.FilterableBooks.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.Book> _bookResourceCollection;

    private readonly IBaseResourceCollectionCount<BL.ResourceEntities.Book>
        _bookResourceCollectionCount;

    private readonly IConfigurationProvider _mapperConfigurationProvider;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.Book> bookResourceCollection,
        IBaseResourceCollectionCount<BL.ResourceEntities.Book> bookResourceCollectionCount,
        IConfigurationProvider mapperConfigurationProvider,
        ILogger<Books.V1_0_0.Read.Controller>? logger = default
    ) : base(logger)
    {
        _bookResourceCollectionCount = bookResourceCollectionCount;
        _bookResourceCollection = bookResourceCollection;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Read filterable books
    /// </summary>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.FilterableBooks.Build)]
    [HttpGet]
    public async Task<IActionResult> ReadWithFiltersApplied(
        [FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _bookResourceCollection
            .ReadSettings(sets => sets
                .ApplyTargetFilters(request)
                .Share(readSettings => _bookResourceCollectionCount
                    .ResourceCollectionSettings(_ => readSettings))
                .ReadPart(request)
                .ProjectTo<ReadResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        ICollection<ReadResponse> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }

        var targetResult = targetResource
            .ToDataPartResponse(request, await _bookResourceCollectionCount.ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}