using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0.Delete;

namespace BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Implementations.Default;

public class DeleteBookResource : IDeleteBookResource
{
    private readonly IBaseResource<ResourceEntities.Book> _bookResource;
    private readonly IMapper _mapper;

    public DeleteBookResource(
        IBaseResource<ResourceEntities.Book> bookResource,
        IMapper mapper
    )
    {
        _bookResource = bookResource;
        _mapper = mapper;
    }

    public async Task<ResultModel> Delete(DeleteRequest request)
        => await DeleteInner(request.ToEntity());

    public async Task<ResultModel> Delete(DeleteBookCommand request)
        => await DeleteInner(request.ToEntity());

    public Task<ResultModel> DeleteInner(ResourceEntities.Book request)
    {
        var targetResource = _bookResource.Delete(request);

        // await _transactionOutboxPublishEndpoint.Publish(_mapper.Map<BookDeleted>(targetResource));

        var targetResourceToResult = _mapper.Map<DeleteResponse>(targetResource);

        return Task.FromResult(ResultModelBuilder
            .Deleted(targetResourceToResult)
            .Build()
            .ToResultModel());
    }
}

public static class UpdateBookRequestExtensions
{
    public static ResourceEntities.Book ToEntity(this DeleteBookCommand request)
        => new()
        {
            BookId = request.BookId
        };
}