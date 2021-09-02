namespace GGroupp.Internal.Support.Bot
{
    public interface IUserLogInConfiguration
    {
        string OAuthConnectionName { get; }

        string OAuthPromptText { get; }

        string OAuthPromptTitle { get; }

        int? OAuthTimeoutMilliseconds { get; }
    }
}