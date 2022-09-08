namespace BookStore.AuthorizationService.Contracts.Account.V1_0_0.Create;

public class CreateRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}