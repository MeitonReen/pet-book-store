namespace BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile
{
    public interface IUserClaimsProfile
    {
        string UserId { get; }
        string Name { get; }
    }
}