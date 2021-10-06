using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    using IUserLogInFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
    using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
    using IIncidentCustomerFindFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentCustomerFindFlowOut>>;
    using IIncidentTitleGetFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;

    internal static class BotDependencyFlow
    {
        public static Dependency<IUserLogInFlowFunc> UseUserLogInFlow(this Dependency<UserState> userStateDependency)
            =>
            BotDependencyApi.UseADUserGetApi()
            .With(
                BotDependencyApi.UseUserGetApi())
            .With(
                BotServiceProvider.GetConfiguration<UserLogInConfiguration>)
            .With(
                userStateDependency)
            .With(
                BotServiceProvider.GetLoggerFactory)
            .UseUserLogInFlow();

        public static Dependency<IIncidentCreateFlowFunc> UseIncidentCreateFlow()
            =>
            BotDependencyApi.UseIncidentCreateApi()
            .With(
                BotServiceProvider.GetLoggerFactory)
            .UseIncidentCreateFlow();

        public static Dependency<IIncidentCustomerFindFlowFunc> UseIncidentCustomerFindFlow()
            =>
            BotDependencyApi.UseCustomerSetFindApi()
            .With(
                BotServiceProvider.GetLoggerFactory)
            .UseIncidentCustomerFindFlow();

        public static Dependency<IIncidentTitleGetFlowFunc> UseIncidentTitleGetFlow()
            =>
            Dependency.Empty.UseIncidentTitleGetFlow();
    }
}