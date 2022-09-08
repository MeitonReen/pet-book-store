using BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Implementations.Default;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Helpers.Abstractions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Abstractions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Implementation;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Abstractions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Implementation;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;
using BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Abstractions;
using BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Implementations.Default;
using BookStore.OrderService.Data.ResourcesReadSettings;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.OrderService.Settings.DiContainer;

public static class BlResourcesDiSettings
{
    public static IServiceCollection AddBlResources(this IServiceCollection services)
        => services
            .AddScoped<IDeleteBookResource, DeleteBookResource>()
            .AddScoped<ICreateMyCartResource, CreateMyCartResource>()
            .AddScoped<ICreateBookInMyCartResource, CreateBookInMyCartResource>()
            .AddScoped<ICreateMyOrderResourceFromMyCart, CreateMyOrderResourceFromMyCart>()
            .AddScoped<IMyCartResourceReadSettings, MyCartResourceReadSettingsByEfCore>()
            .AddScoped<ICartResourceCollectionReadSettings, CartResourceCollectionReadSettings>();
}