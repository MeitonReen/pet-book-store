using System.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.NonAttachedExtensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static void AddDefaultTransactionOutbox<TDbContext>(
        this IBusRegistrationConfigurator busRegistrationConfigurator)
        where TDbContext : DbContext
    {
        busRegistrationConfigurator.AddEntityFrameworkOutbox<TDbContext>(outBoxSets =>
        {
            outBoxSets.UsePostgres();
            outBoxSets.UseBusOutbox();
            outBoxSets.IsolationLevel = IsolationLevel.ReadCommitted;
        });
    }
}