using System.Security.Claims;
using BookStore.AuthorizationService.BL.ResourceEntities;
using BookStore.AuthorizationService.Configs.DefaultClients;
using BookStore.AuthorizationService.Configs.DefaultUsers;
using BookStore.AuthorizationService.Defaults;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Implementations.DatabaseInit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Serilog;

namespace BookStore.AuthorizationService.Data.BaseDatabase.DatabaseInit
{
    public class DatabaseInit : IDatabaseInit
    {
        private readonly BaseDbContext _dbContext;
        private readonly DefaultClientsConfig _defaultClientsConfig;
        private readonly DefaultUsersConfig _defaultUsersConfig;
        private readonly IOpenIddictApplicationManager _openIddictApplicationManager;
        private readonly IOpenIddictScopeManager _openIddictScopeManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<User> _userManager;

        public DatabaseInit(BaseDbContext dbContext, UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOpenIddictApplicationManager openIddictApplicationManager,
            IOpenIddictScopeManager openIddictScopeManager,
            IOptionsSnapshotMixOptionsMonitor<DefaultClientsConfig> defaultClientsConfigAccessor,
            IOptionsSnapshotMixOptionsMonitor<DefaultUsersConfig> defaultUsersConfigAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _openIddictApplicationManager = openIddictApplicationManager;
            _openIddictScopeManager = openIddictScopeManager;
            _defaultClientsConfig = SetAllowedScopesToDefaultClients(
                defaultClientsConfigAccessor.Value);
            _defaultUsersConfig = defaultUsersConfigAccessor.Value;
        }

        public async Task SeedAsync(Func<DbContext, Task>? dbInitSettings = default)
        {
            try
            {
                await _dbContext.Database.MigrateAsync();

                if (dbInitSettings != default)
                {
                    await dbInitSettings(_dbContext);
                    await _dbContext.SaveChangesAsync();

                    return;
                }

                await SeedDefaultScopes();
                await SeedDefaultUsers();
                await SeedDefaultClients();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private DefaultClientsConfig SetAllowedScopesToDefaultClients(
            DefaultClientsConfig defaultClientsConfig)
        {
            defaultClientsConfig.BookStoreSwaggerUiConfig.AllowedScopes = new[]
            {
                BookStoreDefaultScopes.DefaultBookStoreResourcesCRUD,
                BookStoreDefaultScopes.DefaultAdminResourcesCRUD,
                BookStoreDefaultScopes.DefaultUserResourcesCRUD,
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Profile
            };
            return defaultClientsConfig;
        }

        // private async Task SeedDefaultRoles() => await Task.WhenAll(
        //     new[] {DefaultUserRoles.Admin, DefaultUserRoles.DefaultUser}.Select(CreateRoleIfNotExists)
        // );


        private async Task SeedDefaultScopes()
        {
            await new[]
                {
                    BookStoreDefaultScopes.DefaultBookStoreResourcesCRUD,
                    BookStoreDefaultScopes.DefaultAdminResourcesCRUD,
                    BookStoreDefaultScopes.DefaultUserResourcesCRUD
                }
                .ToAsyncEnumerable()
                .ForEachAwaitAsync(async el => await CreateScopeIfNotExists(el));
        }

        private Task SeedDefaultUsers() =>
            new[] {_defaultUsersConfig.Superuser, _defaultUsersConfig.Admin, _defaultUsersConfig.TestDefaultUser}
                .ToAsyncEnumerable()
                .ForEachAwaitAsync(async el =>
                {
                    var createdUser = await ReCreateUser(el.Name, el.Password);

                    await CreateClaimsToUser(createdUser,
                        el.AllowedScopes
                            .Select(el => (OpenIddictConstants.Claims.Private.Scope, el))
                            .ToArray());
                });

        private async Task SeedDefaultClients()
        {
            var bookStoreSwaggerUiConfig = _defaultClientsConfig.BookStoreSwaggerUiConfig;
            var bookServiceSwaggerUiAppDescription = new OpenIddictApplicationDescriptor
            {
                ClientId = bookStoreSwaggerUiConfig.ClientId,
                ClientSecret = bookStoreSwaggerUiConfig.ClientSecret,

                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = bookStoreSwaggerUiConfig.DisplayName,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Code
                }
                // Requirements =
                // {
                //     OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                // }
            };
            bookStoreSwaggerUiConfig.RedirectUris
                .Aggregate(bookServiceSwaggerUiAppDescription.RedirectUris,
                    (aggregator, el) =>
                    {
                        aggregator.Add(new Uri(el));
                        return aggregator;
                    });

            Array.ForEach(
                bookStoreSwaggerUiConfig.AllowedScopes
                    .Select(el => OpenIddictConstants.Permissions.Prefixes.Scope + el)
                    .ToArray(),
                el => bookServiceSwaggerUiAppDescription.Permissions.Add(el));

            await ReCreateClientApplication(bookServiceSwaggerUiAppDescription);
        }

        private async Task CreateScopeIfNotExists(string scope)
        {
            if (await _openIddictScopeManager.FindByNameAsync(scope) != default) return;

            await _openIddictScopeManager.CreateAsync(new OpenIddictScopeDescriptor {Name = scope});
        }

        private async Task ReCreateClientApplication(OpenIddictApplicationDescriptor applicationDescriptor)
        {
            if (applicationDescriptor.ClientId == default) return;

            var client = await _openIddictApplicationManager
                .FindByClientIdAsync(applicationDescriptor.ClientId);

            if (client != default)
            {
                await _openIddictApplicationManager.DeleteAsync(client);
            }

            await _openIddictApplicationManager.CreateAsync(applicationDescriptor);
        }

        private async Task CreateClientApplicationIfNotExists(
            OpenIddictApplicationDescriptor applicationDescriptor)
        {
            if (applicationDescriptor.ClientId == default) return;

            if (await _openIddictApplicationManager.FindByClientIdAsync(applicationDescriptor.ClientId) !=
                default) return;

            await _openIddictApplicationManager.CreateAsync(applicationDescriptor);
        }

        private async Task CreateRoleIfNotExists(string roleName)
        {
            IdentityRole<Guid> role;
            role = await _roleManager.FindByNameAsync(roleName);

            if (role != default)
            {
                Log.Debug($"Role {roleName} already exists");
                return;
            }

            role = new IdentityRole<Guid> {Name = roleName};

            var createRoleResult = await _roleManager.CreateAsync(role);
            if (!createRoleResult.Succeeded)
            {
                throw new Exception(createRoleResult.Errors.First().Description);
            }

            Log.Debug($"Role {roleName} created");
        }

        private async Task<User> ReCreateUser(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != default)
            {
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    throw new Exception(deleteResult.Errors.First().Description);
                }
            }
            else
            {
                user = new User();
            }

            user.UserName = userName;

            var createUserResult = await _userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
            {
                throw new Exception(createUserResult.Errors.First().Description);
            }

            Log.Debug($"{userName} created");

            return user;
        }

        private async Task<User> CreateUserIfNotExists(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != default)
            {
                Log.Debug($"{userName} already exists");
                return user;
            }

            user = new User
            {
                UserName = userName
            };
            var createUserResult = await _userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
            {
                throw new Exception(createUserResult.Errors.First().Description);
            }

            Log.Debug($"{userName} created");

            return user;
        }

        private async Task CreateClaimsToUser(User user, params (string ClaimType, string ClaimValue)[] claims)
        {
            var createClaimsToUserResult = await _userManager.AddClaimsAsync(user,
                claims.Select(el => new Claim(el.ClaimType, el.ClaimValue)));

            if (!createClaimsToUserResult.Succeeded)
            {
                throw new Exception(createClaimsToUserResult.Errors.First().Description);
            }
        }

        private async Task CreateRoleToUser(string userName, string roleName)
        {
            User user;
            user = await _userManager.FindByNameAsync(userName);
            if (user == default)
            {
                throw new Exception($"{nameof(CreateRoleToUser)}: user not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(roleName))
            {
                Log.Debug($"{userName} already has {roleName}");
                return;
            }

            var setRoleToUserResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!setRoleToUserResult.Succeeded)
            {
                throw new Exception(setRoleToUserResult.Errors.First().Description);
            }

            Log.Debug($"Set role {roleName} to {userName} is successful");
        }

        private async Task CreateScopeToRole(string scopeName, string roleName)
        {
            var targetRole = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Name == roleName);
            if (targetRole == default) return;

            await _roleManager.AddClaimAsync(targetRole,
                new Claim(OpenIddictConstants.Claims.Private.Scope, scopeName));
        }

        private async Task CreateClaimToUserIfNotExists(string claimType, string claimValue, string userName)
        {
            var targetUser = await _userManager.FindByNameAsync(userName);
            if (targetUser == default) return;

            var targetUserClaims = await _userManager.GetClaimsAsync(targetUser);
            if (targetUserClaims.Any(claim => claim.Type == claimType && claim.Value == claimValue)) return;

            await _userManager.AddClaimAsync(targetUser, new Claim(claimType, claimValue));
        }

        private async Task CreateResourceToScope(string resourceName, string scopeName)
        {
            if (string.IsNullOrEmpty(scopeName)) return;

            var targetScope = await _openIddictScopeManager.FindByNameAsync(scopeName);
            if (targetScope == default) return;

            var targetScopeDescriptor = new OpenIddictScopeDescriptor();

            await _openIddictScopeManager.PopulateAsync(targetScopeDescriptor, targetScope);

            targetScopeDescriptor.Resources.Add(resourceName);

            await _openIddictScopeManager.PopulateAsync(targetScope, targetScopeDescriptor);
        }
    }
}