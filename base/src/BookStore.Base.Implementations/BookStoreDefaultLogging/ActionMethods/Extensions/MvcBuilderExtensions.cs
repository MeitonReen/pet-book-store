using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BookStoreDefaultLogging.ActionMethods.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddBookStoreDefaultLogging(this IMvcBuilder mvcBuilder) =>
        mvcBuilder.AddMvcOptions(sets =>
            sets.Filters.Add<BookStoreLoggingActionMethodFilter>());
}