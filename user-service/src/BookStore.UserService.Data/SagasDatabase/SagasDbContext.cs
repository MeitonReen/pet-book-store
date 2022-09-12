using BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance;
using BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance.Postgres;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.Data.SagasDatabase;

public class SagasDbContext : SagaDbContext
{
    public SagasDbContext(DbContextOptions<SagasDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new SagaOrchestratorInstanceToDbMapByEfCore(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}