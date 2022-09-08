using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.NotFound.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.Contracts.Author.V1_0_0;
using BookStore.BookService.Contracts.Author.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.BookService.WebEntryPoint.Author.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.Author> _authorResource;
    private readonly IMapper _mapper;

    public Controller(
        IBaseResource<BL.ResourceEntities.Author> authorResource,
        IMapper mapper /*, MinioClient objectManager,*/,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _authorResource = authorResource;
        _mapper = mapper;
    }

    /// <summary>
    /// Read an author
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(ReadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.Author.Build)]
    [HttpGet]
    public async Task<IActionResult> Read([FromQuery] ReadRequest request)
    {
        var configuredResource = _authorResource
            .ReadSettings(sets => sets
                .Where(author => author.AuthorId == request.AuthorId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        return targetResult != default
            ? ResultModelBuilder
                .Read(targetResult)
                .Build()
                .ToActionResult()
            : ResultModelBuilder
                .NotFound()
                .ApplyDefaultSettings()
                .Build()
                .ToActionResult();
    }
}