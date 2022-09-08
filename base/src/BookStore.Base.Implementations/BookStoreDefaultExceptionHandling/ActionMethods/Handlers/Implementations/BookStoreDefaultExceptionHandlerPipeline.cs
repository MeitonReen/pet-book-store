using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Implementations;

public class BookStoreDefaultExceptionHandlerPipeline : IBookStoreDefaultExceptionHandler
{
    private readonly IBookStoreDefaultExceptionHandler[] _exceptionHandlers;

    public BookStoreDefaultExceptionHandlerPipeline(
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigOptionsAccessor
    )
    {
        var environment = appConfigOptionsAccessor.Value.Environment;

        _exceptionHandlers = new IBookStoreDefaultExceptionHandler[]
        {
            new BookStoreDefaultExceptionHandlerDbUpdateConcurrencyException(
                environment),
            new BookStoreDefaultExceptionHandlerDbUpdateException(environment)
        };
    }

    public IActionResult? Handle(Exception exception)
    {
        IActionResult? result = default;
        _exceptionHandlers.FirstOrDefault(handler =>
        {
            result = handler.Handle(exception);
            return result != default;
        });

        return result;
    }
}