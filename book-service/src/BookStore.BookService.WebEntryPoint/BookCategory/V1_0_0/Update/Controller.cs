using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.ResourcesAuthorization.Contracts;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.BookCategory.V1_0_0;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;

namespace BookStore.BookService.WebEntryPoint.BookCategory.V1_0_0.Update;

using static ResourcesPolicyConstants;

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
    /// Update a book category
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
    [Route(HttpApiRouteBuilder.BookCategory.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] [Required] UpdateRequest request)
    {
        var targetResource = _bookCategoryResource.Update(request.ToEntity()).ResourceEntity;

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e) when
            (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<UpdateResponse>(
            targetResource);

        return ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToActionResult();

        // await _cacheInvalidator.InvalidateAsync(this, typeof(BookCategoriesController),
        //     nameof(this.ReadBookCategory), new ReadBookCategoryRequest()
        //         {CategoryId = request.CategoryId});
    }
}

public static class UpdateRequestExtensions
{
    public static BL.ResourceEntities.BookCategory ToEntity(this UpdateRequest request)
        => new()
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description
        };
}