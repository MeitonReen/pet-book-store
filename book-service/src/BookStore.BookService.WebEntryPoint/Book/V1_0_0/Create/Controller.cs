using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.Book.V1_0_0;
using BookStore.BookService.Contracts.Book.V1_0_0.Create;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IMapper _mapper;
    private readonly IResourcesCommitter _resourcesCommitter;

    // private readonly ICacheInvalidator _cacheInvalidator;

    public Controller(
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
        _bookBaseResource = bookBaseResource;
        // _cacheInvalidator = cacheInvalidator;
    }

    /// <summary>
    /// Create a book
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
    [Route(HttpApiRouteBuilder.Book.Build)]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] [Required] CreateRequest request)
    {
        var targetResource = _bookBaseResource.Create(request.ToEntity()).ResourceEntity;

        if (request.AuthorIds.Any())
        {
            var targetAuthorsRefs = request.AuthorIds.Select(authorId =>
                new BL.ResourceEntities.Author {AuthorId = authorId});

            targetResource = _bookBaseResource
                .CreateReferences(book => book.Authors, targetAuthorsRefs)
                .ResourceEntity;
        }

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
}

public static class CreateRequestExtensions
{
    public static BL.ResourceEntities.Book ToEntity(this CreateRequest request)
        => new()
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            PublicationDate = request.PublicationDate
        };
}