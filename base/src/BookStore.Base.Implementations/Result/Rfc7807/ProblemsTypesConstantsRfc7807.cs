using Microsoft.AspNetCore.Http;

namespace BookStore.Base.Implementations.Result.Rfc7807
{
    public static class ProblemsTypesConstantsRfc7807
    {
        public static class BadRequest
        {
            public const string Type = "/bad-request";
            public const string Title = "Bad request";
            public const int Status = StatusCodes.Status400BadRequest;
        }

        public static class Conflict
        {
            public const string Type = "/conflict";
            public const string Title = "Target resource already created";
            public const int Status = StatusCodes.Status409Conflict;
        }

        public static class NotFound
        {
            public const string Type = "/not-found";
            public const string Title = "Target resource not found";
            public const int Status = StatusCodes.Status404NotFound;
        }

        public static class Exception
        {
            public const string Type = "/internal-server-error";
            public const string Title = "Internal server error";
            public const int Status = StatusCodes.Status500InternalServerError;
        }
    }
}