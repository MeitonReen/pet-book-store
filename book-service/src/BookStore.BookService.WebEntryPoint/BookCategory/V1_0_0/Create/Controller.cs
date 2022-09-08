using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.BookCategory.V1_0_0;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.BookService.WebEntryPoint.BookCategory.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.BookCategory> _bookCategoryResource;
    private readonly IMapper _mapper;

    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResource<BL.ResourceEntities.BookCategory> bookCategoryResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookCategoryResource = bookCategoryResource;
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
        // _cacheInvalidator = cacheInvalidator;
    }

    /// <summary>
    /// Create a book category
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{C}"
               + Or +
               $"{DefaultBookStoreResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookCategory.Build)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] [Required] CreateRequest request)
    {
        var targetResource = _bookCategoryResource.Create(request.ToEntity()).ResourceEntity;

        try
        {
            await _resourcesCommitter.CommitAsync();
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

    /// <summary>
    /// Create a book reference in a category
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{C}"
               + Or +
               $"{DefaultBookStoreResources}{C}")]
    [ProducesResponseType(typeof(CreateBookReferenceResponse), StatusCodes.Status201Created)]
    [Route(HttpApiRouteBuilder.BookCategory.BookRef.Build)]
    [HttpPost]
    public async Task<IActionResult> CreateBookRef([FromBody] [Required] CreateBookReferenceRequest request)
    {
        _bookCategoryResource.CreateReference(
            bookCategory => bookCategory.CategoryId = request.CategoryId,
            bookCategory => bookCategory.Books,
            book => book.BookId = request.BookId);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<CreateBookReferenceResponse>(request);

        return ResultModelBuilder
            .Created(targetResourceToResult)
            .Build()
            .ToActionResult();
    }
}

public static class CreateRequestExtensions
{
    public static BL.ResourceEntities.BookCategory ToEntity(this CreateRequest request)
        => new()
        {
            Name = request.Name,
            Description = request.Description
        };
}