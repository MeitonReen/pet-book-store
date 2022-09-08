using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance;

public class SagaOrchestratorInstanceToDbMap : SagaClassMap<SagaOrchestratorInstance>
{
    protected override void Configure(EntityTypeBuilder<SagaOrchestratorInstance> entity,
        ModelBuilder model)
    {
        var sagaInstanceFullName = typeof(SagaOrchestratorInstance).FullName;
        if (sagaInstanceFullName == default)
            throw new InvalidOperationException($"{nameof(sagaInstanceFullName)} not found");

        entity.ToTable(new string(
            sagaInstanceFullName
                .TakeLast(63) //postgres table name max
                .ToArray())
        );

        entity.Property(sets => sets.CurrentState);
        entity.Property(sets => sets.ResponseAddress);
        entity.Property(sets => sets.RequestId);
        entity.Property(sets => sets.OrchestratorInstanceAddress);

        entity
            .Property(sets => sets.ConcurrencyToken)
            .HasColumnType("xid")
            .HasColumnName("xmin")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}