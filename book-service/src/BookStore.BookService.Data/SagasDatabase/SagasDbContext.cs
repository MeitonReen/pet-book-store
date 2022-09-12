using BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance;
using BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance.Postgres;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BookService.Data.SagasDatabase;

public class SagasDbContext : SagaDbContext
{
    public SagasDbContext(DbContextOptions<SagasDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new SagaOrchestratorInstanceToDbMapByEfCore();
            yield return new Book.V1_0_0.UpdateOut.SagaInstance.Postgres.SagaOrchestratorInstanceToDbMapByEfCore();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}