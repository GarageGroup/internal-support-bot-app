using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetProjects(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmProjectApi crmProjectApi)
        =>
        chatFlow.NextValue(
            crmProjectApi.GetProjectsAsync);

    private static async ValueTask<IncidentCreateFlowState> GetProjectsAsync(
        this ICrmProjectApi crmProjectApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.Customer is null || context.FlowState.Project is not null ||
            context.FlowState.IsProjectSkipped || context.FlowState.FoundProjects.IsNotEmpty)
        {
            return context.FlowState;
        }

        var result = await crmProjectApi.InnerGetProjectsAsync(context, cancellationToken).ConfigureAwait(false);

        if (result is null)
        {
            return context.FlowState;
        }

        var projects = result.Value.Projects.Map(MapProject);

        if (projects.IsEmpty)
        {
            await context.Api.SendHtmlModeTextAndRemoveReplyKeyboardAsync(
                context.Localizer[NotExistProjectsText], cancellationToken).ConfigureAwait(false);

            return context.FlowState with
            {
                FoundProjects = projects,
                IsProjectSkipped = true
            };
        }

        return context.FlowState with
        {
            FoundProjects = projects
        };
    }

    private static Task<ProjectSetGetOut?> InnerGetProjectsAsync(
        this ICrmProjectApi crmProjectApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState.Customer?.Id, cancellationToken)
        .Pipe(
            static customerId => new ProjectSetGetIn(customerId.GetValueOrDefault()))
        .PipeValue(
            crmProjectApi.GetAsync)
        .OnFailure(
            failure => context.Logger.LogError(failure.SourceException, "Get projects failure: {message}", failure.FailureMessage))
        .Fold<ProjectSetGetOut?>(
            static success => success,
            static _ => null);

    private static IncidentProjectState MapProject(ProjectItemOut project)
        =>
        new(
            id: project.Id,
            name: project.Name);
}
