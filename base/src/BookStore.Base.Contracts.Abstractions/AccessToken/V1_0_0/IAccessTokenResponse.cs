namespace BookStore.Base.Contracts.Abstractions.AccessToken.V1_0_0;

public interface IAccessTokenResponse
{
    string access_token { get; set; }
    long expires_in { get; set; }
    string token_type { get; set; }
    string scope { get; set; }
}