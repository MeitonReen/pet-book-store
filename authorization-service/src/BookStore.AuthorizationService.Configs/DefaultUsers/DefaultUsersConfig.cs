using BookStore.AuthorizationService.Defaults;
using OpenIddict.Abstractions;

namespace BookStore.AuthorizationService.Configs.DefaultUsers;

public class DefaultUsersConfig
{
    public DefaultUserConfig Superuser { get; init; } =
        new(nameof(Superuser), new[]
        {
            BookStoreDefaultScopes.DefaultBookStoreResourcesCRUD,
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile
        });

    public DefaultUserConfig Admin { get; init; } =
        new(nameof(Admin), new[]
        {
            BookStoreDefaultScopes.DefaultAdminResourcesCRUD,
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile
        });

    public DefaultUserConfig TestDefaultUser { get; init; } =
        new(nameof(TestDefaultUser), new[]
        {
            BookStoreDefaultScopes.DefaultUserResourcesCRUD,
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile
        });

    public static DefaultUsersConfig Empty => new();
}