using BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Implementations.Default;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Implementations.Default;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.BookService.Settings.DiContainer;

public static class BlResourcesDiSettings
{
    public static IServiceCollection AddBlResources(this IServiceCollection services)
        => services
            .AddScoped<IDeleteBookResource, DeleteBookResource>()
            .AddScoped<IUpdateBookResource, UpdateBookResource>();
}