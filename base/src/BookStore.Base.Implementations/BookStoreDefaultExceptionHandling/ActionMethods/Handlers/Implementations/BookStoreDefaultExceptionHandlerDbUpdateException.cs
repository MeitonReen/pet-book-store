using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Abstractions;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Implementations;

public class BookStoreDefaultExceptionHandlerDbUpdateException : IBookStoreDefaultExceptionHandler
{
    private readonly string _environment;

    public BookStoreDefaultExceptionHandlerDbUpdateException(
        string environment
    )
    {
        _environment = environment;
    }

    public IActionResult? Handle(Exception exception)
    {
        if (exception is not DbUpdateException internalException) return default;

        return ResultModelBuilder
            .BadRequest()
            .ApplyDefaultSettings(
                $"Exception: {internalException.Message};"
                + $" Inner exception: {internalException.InnerException?.Message}")
            .Environment(_environment)
            .Build()
            .ToActionResult();
    }
}