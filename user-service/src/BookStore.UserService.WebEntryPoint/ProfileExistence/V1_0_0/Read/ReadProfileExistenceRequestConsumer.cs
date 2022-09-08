using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.ProfileExistence.V1_0_0.Read;
using MassTransit;

namespace BookStore.UserService.WebEntryPoint.ProfileExistence.V1_0_0.Read;

public class ReadProfileExistenceRequestConsumer : IConsumer<ReadProfileExistenceRequest>
{
    private readonly IBaseResourceExistence<BL.ResourceEntities.Profile> _profilePresence;

    public ReadProfileExistenceRequestConsumer(
        IBaseResourceExistence<BL.ResourceEntities.Profile> profilePresence
    )
    {
        _profilePresence = profilePresence;
    }

    public async Task Consume(ConsumeContext<ReadProfileExistenceRequest> context)
    {
        var configuredResource = _profilePresence
            .ReadSettings(profile => profile.UserId = context.Message.UserId);

        bool targetResourceIsPresentInResourceCollection;
        try
        {
            targetResourceIsPresentInResourceCollection = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResourceIsPresentInResourceCollection = await configuredResource.ReadAsync();
        }

        if (targetResourceIsPresentInResourceCollection)
        {
            await context.RespondAsync<Found>(new { });
        }
        else
        {
            await context.RespondAsync<NotFound>(new { });
        }
    }
}