namespace BookStore.Base.Implementations.__Obsolete.Authorization;

public static class PolicyConstantBuilder
{
    public static class Admin
    {
        public const string Build = "Admin";

        public static class Or
        {
            private const string Build = "AdminOr";

            public static class AuthenticatedUser
            {
                public const string Build = "AdminOrAuthenticatedUser";
            }
        }
    }

    public static class AuthenticatedUser
    {
        public const string Build = "AuthenticatedUser";
    }
}