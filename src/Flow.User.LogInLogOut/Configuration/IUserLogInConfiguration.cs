using System;

namespace GGroupp.Internal.Support.Bot;

public interface IUserLogInConfiguration
{
    string OAuthConnectionName { get; }

    int? OAuthTimeoutMilliseconds { get; }
}
