namespace BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile
{
    public interface IUserClaimsProfileSetter
    {
        string UserId { get; set; }
        string Name { get; set; }
    }
}