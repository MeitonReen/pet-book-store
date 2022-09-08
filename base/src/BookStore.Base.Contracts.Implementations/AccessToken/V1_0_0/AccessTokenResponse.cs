using BookStore.Base.Contracts.Abstractions.AccessToken.V1_0_0;

namespace BookStore.Base.Contracts.Implementations.AccessToken.V1_0_0;

public class AccessTokenResponse : IAccessTokenResponse
{
    public string access_token { get; set; } = string.Empty;
    public long expires_in { get; set; }
    public string token_type { get; set; } = string.Empty;
    public string scope { get; set; } = string.Empty;
}