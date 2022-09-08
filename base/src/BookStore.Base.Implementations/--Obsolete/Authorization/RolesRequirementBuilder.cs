using Microsoft.AspNetCore.Authorization;

namespace BookStore.Base.Implementations.__Obsolete.Authorization
{
    public class RolesRequirementBuilder
    {
        private readonly List<string> _roles = new();
        private Func<IAuthorizationRequirement> _createInstance;

        public RolesRequirementBuilder Or
        {
            get
            {
                _createInstance ??= () => new RolesRequirementOr(_roles.ToArray());
                return this;
            }
        }

        public RolesRequirementBuilder Role(string role)
        {
            _roles.Add(role);
            return this;
        }

        public IAuthorizationRequirement Build() => _createInstance();
    }
}