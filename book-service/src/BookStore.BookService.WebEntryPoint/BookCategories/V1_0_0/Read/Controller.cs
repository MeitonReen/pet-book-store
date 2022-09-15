using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.BookCategories.V1_0_0;
using BookStore.BookService.Contracts.BookCategories.V1_0_0.Read;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.BookService.WebEntryPoint.BookCategories.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookCategory>
        _bookCategoryResourceCollection;

    private readonly IBaseResourceCollectionCount<BL.ResourceEntities.BookCategory>
        _bookCategoryResourceCollectionCount;

    private readonly IConfigurationProvider _mapperConfigurationProvider;
    // private readonly ICacheInvalidator _cacheInvalidator;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.BookCategory> bookCategoryResourceCollection,
        IBaseResourceCollectionCount<BL.ResourceEntities.BookCategory> bookCategoryResourceCollectionCount,
        IConfigurationProvider mapperConfigurationProvider,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookCategoryResourceCollectionCount = bookCategoryResourceCollectionCount;
        _bookCategoryResourceCollection = bookCategoryResourceCollection;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Read book categories
    /// </summary>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookCategories.Build)]
    [HttpGet]
    public async Task<IActionResult> Read([FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _bookCategoryResourceCollection
            .ReadSettings(sets => sets
                .OrderBy(bookCategory => bookCategory.CategoryId)
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
            .ToDataPartResponse(request, await _bookCategoryResourceCollectionCount.ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}