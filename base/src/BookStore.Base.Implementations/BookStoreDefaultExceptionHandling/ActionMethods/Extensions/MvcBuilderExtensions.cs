using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Abstractions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Handlers.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddBookStoreDefaultExceptionHandling(
        this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddMvcOptions(sets =>
            sets.Filters.Add<BookStoreDefaultExceptionHandlingFilter>());
        mvcBuilder.Services.AddScoped<
            IBookStoreDefaultExceptionHandler, BookStoreDefaultExceptionHandlerPipeline>();

        return mvcBuilder;
    }
}