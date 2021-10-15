using System;

namespace GGroupp.Internal.Support.Bot;

public sealed record ADUserGetIn
{
    public ADUserGetIn(string accessToken) => AccessToken = accessToken.OrEmpty();

    public string AccessToken { get; }
}
