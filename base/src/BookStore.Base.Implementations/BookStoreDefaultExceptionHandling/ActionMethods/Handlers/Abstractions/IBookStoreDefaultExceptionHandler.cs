using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Abstractions;

public interface IBookStoreDefaultExceptionHandler
{
    IActionResult? Handle(Exception exception);
}