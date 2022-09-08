using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;

namespace BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default;

public class UserClaimsProfileSetter : IUserClaimsProfileSetter
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}