using System;

namespace GGroupp.Internal.Support.Bot;

internal sealed partial record UserLogInConfiguration : IUserLogInConfiguration
{
    public string? OAuthConnectionName { get; init; }

    public string? OAuthPromptText { get; init; }

    public string? OAuthPromptTitle { get; init; }

    public int? OAuthTimeoutMilliseconds { get; init; }

    string IUserLogInConfiguration.OAuthConnectionName => OAuthConnectionName.OrEmpty();

    string IUserLogInConfiguration.OAuthPromptText => OAuthPromptText.OrEmpty();

    string IUserLogInConfiguration.OAuthPromptTitle => OAuthPromptTitle.OrEmpty();

    int? IUserLogInConfiguration.OAuthTimeoutMilliseconds => OAuthTimeoutMilliseconds;
}
