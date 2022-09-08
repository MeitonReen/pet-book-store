using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.NotFound.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.BookRating.V1_0_0;
using BookStore.UserService.Contracts.BookRating.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.UserService.WebEntryPoint.BookRating.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IMapper _mapper;

    public Controller(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        IMapper mapper,
        ILogger<Microsoft.AspNetCore.Mvc.Controller>? logger = default
    ) : base(logger)
    {
        _bookRatingResource = bookRatingResource;
        _mapper = mapper;
    }

    /// <summary>
    /// Read a book rating
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(ReadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookRating.Build)]
    [HttpGet]
    public async Task<IActionResult> Read([FromQuery] ReadRequest request)
    {
        var configuredResource = _bookRatingResource
            .ReadSettings(settings => settings
                .Where(bookRating => bookRating.RatingId == request.RatingId)
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