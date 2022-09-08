namespace BookStore.OrderService.Contracts.ProfileExistence.V1_0_0.ReadOut;

public class ReadProfileExistenceRequest : Base.InterserviceContracts.UserService.V1_0_0.ProfileExistence.V1_0_0.Read.
    ReadProfileExistenceRequest
{
    public Guid UserId { get; set; }
}