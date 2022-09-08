using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Abstractions;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Exception.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods;

public class BookStoreDefaultExceptionHandlingFilter : IExceptionFilter
{
    private readonly IBookStoreDefaultExceptionHandler _bookStoreDefaultExceptionHandler;
    private readonly string _environment;

    public BookStoreDefaultExceptionHandlingFilter(
        IBookStoreDefaultExceptionHandler bookStoreDefaultExceptionHandler,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigOptionsAccessor
    )
    {
        _environment = appConfigOptionsAccessor.Value.Environment;
        _bookStoreDefaultExceptionHandler = bookStoreDefaultExceptionHandler;
    }

    public void OnException(ExceptionContext context)
    {
        var result = _bookStoreDefaultExceptionHandler.Handle(context.Exception);

        if (result != default)
        {
            context.Result = result;
            context.ExceptionHandled = true;
            return;
        }

        context.Result = ResultModelBuilder
            .Exception()
            .ApplyDefaultSettings(
                $"Exception: {context.Exception.Message};"
                + $" Inner exception: {context.Exception.InnerException?.Message}")
            .Environment(_environment)
            .Build()
            .ToActionResult();
        context.ExceptionHandled = true;
    }
}