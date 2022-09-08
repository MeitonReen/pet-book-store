using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReadBookResponse = BookStore.BookService.Contracts.Book.V1_0_0.ReadBookResponse;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Read;

public class ReadBookRequestFromOrderServiceConsumer : IConsumer<ReadBookRequest>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IConfigurationProvider _mapperConfigurationProvider;

    public ReadBookRequestFromOrderServiceConsumer(
        IConfigurationProvider mapperConfigurationProvider,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource
    )
    {
        _mapperConfigurationProvider = mapperConfigurationProvider;
        _bookBaseResource = bookBaseResource;
    }

    public async Task Consume(ConsumeContext<ReadBookRequest> context)
    {
        var configuredResource = _bookBaseResource
            .ReadSettings(settings => settings
                .Where(book => book.BookId == context.Message.BookId)
                .ProjectTo<ReadBookResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        ReadBookResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        if (targetResult == default)
        {
            await context.RespondAsync<NotFound>(new { });
            return;
        }

        await context.RespondAsync(targetResult);
    }
}