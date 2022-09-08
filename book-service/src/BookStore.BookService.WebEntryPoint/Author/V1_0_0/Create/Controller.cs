using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.Author.V1_0_0;
using BookStore.BookService.Contracts.Author.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.BookService.WebEntryPoint.Author.V1_0_0.Create;

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
    /// Create an author
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
    [Route(HttpApiRouteBuilder.Author.Build)]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] [Required] CreateRequest request)
    {
        var targetResource = _authorResource.Create(request.ToEntity()).ResourceEntity;

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
    public static BL.ResourceEntities.Author ToEntity(this CreateRequest request)
        => new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic,
            BirthDate = request.BirthDate
        };
}