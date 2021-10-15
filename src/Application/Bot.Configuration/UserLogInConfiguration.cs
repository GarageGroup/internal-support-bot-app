using System;

namespace GGroupp.Internal.Support.Bot;

internal sealed record UserLogInConfiguration : IUserLogInConfiguration
{
    public string? OAuthConnectionName { get; init; }

    public int? OAuthTimeoutMilliseconds { get; init; }

    string IUserLogInConfiguration.OAuthConnectionName => OAuthConnectionName.OrEmpty();

    int? IUserLogInConfiguration.OAuthTimeoutMilliseconds => OAuthTimeoutMilliseconds;
}
