using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;
using MassTransit;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteProfileCommandActivityDefinition :
    ActivityDefinition<DeleteProfileCommandActivity, DeleteProfileCommand, BL.ResourceEntities.Profile>
{
    public DeleteProfileCommandActivityDefinition()
    {
        ExecuteEndpointName = DeleteProfileCommandActivityContracts.ExecuteEndpoint.Name;
    }
}