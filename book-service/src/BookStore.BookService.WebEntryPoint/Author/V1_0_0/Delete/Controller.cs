using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.Author.V1_0_0;
using BookStore.BookService.Contracts.Author.V1_0_0.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.BookService.WebEntryPoint.Author.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.Author> _authorResource;
    private readonly IMapper _mapper;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResource<BL.ResourceEntities.Author> authorResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper /*, MinioClient objectManager,*/,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _authorResource = authorResource;
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
    }

    /// <summary>
    /// Delete an author
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultAdminResources}{D}"
               + Or +
               $"{DefaultBookStoreResources}{D}")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Author.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] DeleteRequest request)
    {
        var targetResource = _authorResource.Delete(author => author.AuthorId = request.AuthorId);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<DeleteResponse>(targetResource);

        return ResultModelBuilder
            .Deleted(targetResourceToResult)
            .Build()
            .ToActionResult();
        // await _cacheInvalidator.InvalidateAsync(this, typeof(AuthorsController),
        //     nameof(this.ReadAuthor), new ReadAuthorRequest() {AuthorId = request.AuthorId});
    }
}