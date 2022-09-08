using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultUserClaimsProfile(
            this IServiceCollection services)
            => services
                .AddScoped<IUserClaimsProfile, UserClaimsProfile>()
                .AddScoped<IUserClaimsProfileSetter, UserClaimsProfileSetter>();
    }
}