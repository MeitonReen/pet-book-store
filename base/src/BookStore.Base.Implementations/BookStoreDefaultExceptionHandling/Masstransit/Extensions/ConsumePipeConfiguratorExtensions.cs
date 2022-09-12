using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.Masstransit.Extensions;

public static class ConsumePipeConfiguratorExtensions
{
    public static void UseBookStoreDefaultExceptionHandling(this IConsumePipeConfigurator
        consumePipeConfigurator)
    {
        consumePipeConfigurator.UseMessageRetry(retrySets =>
        {
            retrySets.Intervals(100, 500);

            retrySets.Handle<DbUpdateConcurrencyException>();

            retrySets.Handle<InvalidOperationException>(invalidOperationExcSpec =>
                invalidOperationExcSpec.InnerException is NpgsqlException
                    or DbUpdateException {InnerException: NpgsqlException}
            );

            retrySets.Handle<DbUpdateException>(dbUpdateExcSpec =>
                dbUpdateExcSpec.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.TransactionRollback
                    or PostgresErrorCodes.TransactionIntegrityConstraintViolation
                    or PostgresErrorCodes.SerializationFailure
                    or PostgresErrorCodes.StatementCompletionUnknown
                    or PostgresErrorCodes.DeadlockDetected
                }
            );
        });
    }
}