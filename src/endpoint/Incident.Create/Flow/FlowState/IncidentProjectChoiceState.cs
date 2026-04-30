namespace GarageGroup.Internal.Support;

internal sealed record class IncidentProjectChoiceState
{
    public IncidentProjectChoiceState(IncidentProjectState? project)
        =>
        Project = project;

    public IncidentProjectState? Project { get; }
}
