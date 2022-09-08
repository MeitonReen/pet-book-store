using Microsoft.AspNetCore.Authorization;

namespace BookStore.Base.Implementations.__Obsolete.Authorization
{
    public class RolesRequirementOr : IAuthorizationRequirement
    {
        public RolesRequirementOr(params string[] roles)
        {
            Roles = roles;
        }

        public string[] Roles { get; set; }
    }
}