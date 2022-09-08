using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;

public static class BookStoreExceptionHandlingDefaults
{
    public static class WhenExpressions
    {
        public static bool PostgresUniqueViolation(Exception e)
            => e is DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                }
            };

        public static bool RepeatRead(Exception e)
            => e is InvalidOperationException
            {
                InnerException: NpgsqlException or DbUpdateException
                {
                    InnerException: NpgsqlException
                }
            };

        public static bool RepeatCommit(Exception e)
            => e is InvalidOperationException
                {
                    InnerException: NpgsqlException or DbUpdateException
                    {
                        InnerException: NpgsqlException
                    }
                }
                or DbUpdateException
                {
                    InnerException: PostgresException
                    {
                        SqlState: PostgresErrorCodes.TransactionRollback
                        or PostgresErrorCodes.TransactionIntegrityConstraintViolation
                        or PostgresErrorCodes.SerializationFailure
                        or PostgresErrorCodes.StatementCompletionUnknown
                        or PostgresErrorCodes.DeadlockDetected
                    }
                };
    }
}