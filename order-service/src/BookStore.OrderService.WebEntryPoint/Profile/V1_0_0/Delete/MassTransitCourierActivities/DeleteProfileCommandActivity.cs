using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.OrderService.BL.ResourceEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteProfileCommandActivity : IActivity<DeleteProfileCommand, BL.ResourceEntities.Profile>
{
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileBaseResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteProfileCommandActivity(
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Profile> profileBaseResource
    )
    {
        _profileBaseResource = profileBaseResource;
        _resourcesCommitter = resourcesCommitter;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeleteProfileCommand> context)
    {
        var oldEntityState = await _profileBaseResource
            .ReadSettings(settings => settings
                .Where(profile => profile.UserId == context.Arguments.UserId)
                .Include(profile => profile.Carts)
                .Include(profile => profile.Orders)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityState == default) return context.Completed();

        ClearCircularReferences(oldEntityState);

        _profileBaseResource.Delete(profile => profile.UserId = context.Arguments.UserId);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Completed(oldEntityState);
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BL.ResourceEntities.Profile> context)
    {
        var oldEntityState = context.Log;

        CompensateCompositeEntityAfterDelete(oldEntityState);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Compensated();
    }

    private void CompensateCompositeEntityAfterDelete(BL.ResourceEntities.Profile entity)
    {
        var carts = entity.Carts;
        var orders = entity.Orders;

        entity.Carts = new List<Cart>();
        entity.Orders = new List<Order>();

        _profileBaseResource
            .Create(entity)
            .CreateReferences(profile => profile.Carts, carts)
            .CreateReferences(author => author.Orders, orders);
    }

    private void ClearCircularReferences(BL.ResourceEntities.Profile profile)
    {
        profile.Carts = profile.Carts
            .Select(cart =>
            {
                cart.Profile = default!;
                return cart;
            })
            .ToList();

        profile.Orders = profile.Orders
            .Select(order =>
            {
                order.Profile = default!;
                return order;
            })
            .ToList();
    }
}