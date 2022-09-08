using BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Abstractions;
using BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Helpers.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Helpers.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Implementations.Default;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Implementations.Default;
using BookStore.UserService.Data.ResourcesReadSettings;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.UserService.Settings.DiContainer;

public static class BlResourcesDiSettings
{
    public static IServiceCollection AddBlResources(this IServiceCollection services)
        => services
            .AddScoped<IUpdateMyBookRatingResource, UpdateMyBookRatingResource>()
            .AddScoped<IMyBookRatingResourceReadSettings, MyBookRatingResourceReadSettingsByEfCore>()
            .AddScoped<IReadShortMyBookRatingResource, ReadShortMyBookRatingResource>()
            .AddScoped<ICreateMyBookRatingResource, CreateMyBookRatingResource>()
            .AddScoped<IUpdateMyBookReviewResource, UpdateMyBookReviewResource>()
            .AddScoped<IMyBookReviewResourceReadSettings, MyBookReviewResourceReadSettingsByEfCore>()
            .AddScoped<IReadShortMyBookReviewResource, ReadShortMyBookReviewResource>()
            .AddScoped<ICreateMyBookReviewResource, CreateMyBookReviewResource>()
            .AddScoped<ICreateIfNotCreatedBookResource, CreateIfNotCreatedBookResource>();
}