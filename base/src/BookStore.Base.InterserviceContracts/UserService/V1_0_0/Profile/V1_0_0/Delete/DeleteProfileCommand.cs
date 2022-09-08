namespace BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;

public interface DeleteProfileCommand
{
    public Guid UserId { get; set; }
}