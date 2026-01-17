namespace Application.Shared;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime ExpireDate { get; set; }
    public string RefreshToken { get; set; }
}
