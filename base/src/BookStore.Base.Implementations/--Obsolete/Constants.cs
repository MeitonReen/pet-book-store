namespace BookStore.Base.Implementations.__Obsolete
{
    public static class Constants
    {
        public static class EnvironmentNames
        {
            public const string Development = nameof(Development);
            public const string Production = nameof(Production);
        }

        public static class CorrelationId
        {
            public const string StorageHeaderDefault = "X-Correlation-ID";
        }

        public static class Swagger
        {
            public static class Titles
            {
                public const string BookStore = nameof(BookStore);
                public const string BookService = $"{BookStore}.{nameof(BookService)}";
                public const string UserService = $"{BookStore}.{nameof(UserService)}";
                public const string OrderService = $"{BookStore}.{nameof(OrderService)}";
            }
        }

        public static class BindingSource
        {
            public const string Body = "Body";
        }

        public static class Authentication
        {
            private const string AccessDescriptionPrefix = "Access to";

            public static class AudienceSelector
            {
                public static class BookStoreApi
                {
                    public const string Select = nameof(BookStoreApi);
                    public const string Description = $"{AccessDescriptionPrefix} {Select}";
                }

                public static class BookStore
                {
                    private const string AccessDescriptionPrefix =
                        $"{Authentication.AccessDescriptionPrefix} {nameof(BookStore)}";

                    public static class AuthorizationServiceApi
                    {
                        public const string Select = nameof(AuthorizationServiceApi);
                        public const string Description = $"{AccessDescriptionPrefix}.{Select}";
                    }

                    public static class BookServiceApi
                    {
                        public const string Select = nameof(BookServiceApi);
                        public const string Description = $"{AccessDescriptionPrefix}.{Select}";
                    }

                    public static class UserServiceApi
                    {
                        public const string Select = nameof(UserServiceApi);
                        public const string Description = $"{AccessDescriptionPrefix}.{Select}";
                    }

                    public static class OrderServiceApi
                    {
                        public const string Select = nameof(OrderServiceApi);
                        public const string Description = $"{AccessDescriptionPrefix}.{Select}";
                    }
                }
            }

            public static class ScopeSelector
            {
                public static class UserProfile
                {
                    public const string Select = nameof(UserProfile);
                    public const string Description = $"{AccessDescriptionPrefix} {Select}";
                }

                public static class UserRoles
                {
                    public const string Select = nameof(UserRoles);
                    public const string Description = $"{AccessDescriptionPrefix} {Select}";
                }

                public static class ReadProfile
                {
                    public const string Select = nameof(ReadProfile);
                    public const string Description = $"{AccessDescriptionPrefix} {Select}";
                }
            }
        }

        public static class Authorization
        {
            public const string TokenEndpoint = "/connect/token";

            public static class Types
            {
                public const string OAuth2 = "oauth2";
            }

            public static class PolicyBuilder
            {
                public static class Admin
                {
                    public const string Build = nameof(Admin);

                    public static class Or
                    {
                        private const string Build = Admin.Build + nameof(Or);

                        public static class AuthenticatedUser
                        {
                            public const string Build = Or.Build + nameof(AuthenticatedUser);
                        }
                    }
                }

                public static class AuthenticatedUser
                {
                    public const string Build = nameof(AuthenticatedUser);
                }
            }

            public static class Roles
            {
                public const string Admin = nameof(Admin);
                public const string AuthenticatedUser = nameof(AuthenticatedUser);
            }
        }
    }
}