using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectProjectOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ExpectChoiceValueOrSkip(
            CreateProjectStepOption);

    private static ChoiceStepOption<IncidentCreateFlowState, IncidentProjectChoiceState>? CreateProjectStepOption(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.Project is not null || context.FlowState.IsProjectSkipped || context.FlowState.FoundProjects.IsEmpty)
        {
            return null;
        }

        return new(
            choiceSetFactory: GetProjectsAsync,
            resultMessageFactory: CreateResultMessage,
            selectedItemMapper: MapFlowState);

        ValueTask<Result<ChoiceStepSet<IncidentProjectChoiceState>, ChatRepeatState>> GetProjectsAsync(
            ChoiceStepRequest request, CancellationToken _)
            =>
            new(context.CreateProjectChoiceSet(request.Text));

        ChatMessageSendRequest CreateResultMessage(ChoiceStepItem<IncidentProjectChoiceState> item)
        {
            var projectName = item.Value.Project?.Name ?? context.Localizer[SkipProjectButtonText];

            return new(context.BuildProjectResultMessage(projectName))
            {
                ReplyMarkup = new BotReplyKeyboardRemove()
            };
        }

        IncidentCreateFlowState MapFlowState(ChoiceStepItem<IncidentProjectChoiceState> item)
            =>
            context.FlowState with
            {
                Project = item.Value.Project,
                IsProjectSkipped = item.Value.Project is null
            };
    }

    private static ChoiceStepSet<IncidentProjectChoiceState> CreateProjectChoiceSet(
        this IChatFlowContext<IncidentCreateFlowState> context, string? searchText)
    {
        var projects = context.FlowState.FoundProjects.SearchProjects(searchText);

        return new(context.BuildProjectChoiceText(projects))
        {
            Items = projects
                .AsEnumerable()
                .Take(MaxProjectSetCount)
                .Select(context.MapProject)
                .ToFlatArray()
                .Concat(context.GetSkipProjectButton())
        };
    }

    private static FlatArray<IncidentProjectState> SearchProjects(
        this FlatArray<IncidentProjectState> projects, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return projects;
        }

        return projects
            .AsEnumerable()
            .Where(project => project.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToFlatArray();
    }

    private static string BuildProjectChoiceText(
        this IChatFlowContext<IncidentCreateFlowState> context, FlatArray<IncidentProjectState> projects)
    {
        if (projects.IsEmpty)
        {
            return context.Localizer[NotFoundProjectsText];
        }

        if (projects.Length > MaxProjectSetCount)
        {
            return context.Localizer[ChooseOrSearchProjectText];
        }

        return context.Localizer[ChooseProjectText];
    }

    private static ChoiceStepItem<IncidentProjectChoiceState> MapProject(
        this IChatFlowContext<IncidentCreateFlowState> context, IncidentProjectState project)
        =>
        new(
            id: project.Id.ToString(),
            title: project.Name,
            value: new(project));

    private static ChoiceStepItem<IncidentProjectChoiceState> GetSkipProjectButton(
        this IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new(
            id: SkipButtonId,
            title: context.Localizer[SkipProjectButtonText],
            value: new(null));
}
