namespace GGroupp.Internal.Support.Bot;

public sealed record ADUserGetIn
{
    public ADUserGetIn(string accessToken) => AccessToken = accessToken ?? string.Empty;
    
    public string AccessToken { get; }
}