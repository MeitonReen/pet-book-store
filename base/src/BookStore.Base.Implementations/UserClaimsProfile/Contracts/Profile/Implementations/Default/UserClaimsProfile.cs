using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;

namespace BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default
{
    public class UserClaimsProfile : IUserClaimsProfile
    {
        private readonly IUserClaimsProfileSetter _storage;

        public UserClaimsProfile(IUserClaimsProfileSetter storage)
        {
            _storage = storage;
        }

        public string UserId => _storage.UserId;
        public string Name => _storage.Name;
    }
}