using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Implementations.Default;

public class UpdateBookResource : IUpdateBookResource
{
    private readonly IBaseResource<ResourceEntities.Book> _bookResource;
    private readonly IMapper _mapper;

    public UpdateBookResource(
        IBaseResource<ResourceEntities.Book> bookResource,
        IMapper mapper
    )
    {
        _bookResource = bookResource;
        _mapper = mapper;
    }

    public async Task<ResultModel> Update(UpdateRequest request)
        => await UpdateInner(request.ToEntity());

    public async Task<ResultModel> Update(UpdateBookCommand request)
        => await UpdateInner(request.ToEntity());

    private Task<ResultModel> UpdateInner(ResourceEntities.Book targetResource)
    {
        targetResource = _bookResource.Update(targetResource).ResourceEntity;

        var targetResourceToResult = _mapper.Map<UpdateResponse>(targetResource);

        return Task.FromResult(ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToResultModel());
    }
}

public static class UpdateBookRequestMessageExtensions
{
    public static ResourceEntities.Book ToEntity(this UpdateRequest request)
        => new()
        {
            BookId = request.BookId,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            PublicationDate = request.PublicationDate
        };
}

public static class UpdateBookRequestExtensions
{
    public static ResourceEntities.Book ToEntity(this UpdateBookCommand request)
        => new()
        {
            BookId = request.BookId,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            PublicationDate = request.PublicationDate
        };
}