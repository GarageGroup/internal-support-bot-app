using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class Application
{
    private static IBotBuilder UseLogoutFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseLogout(LogoutCommand);
}