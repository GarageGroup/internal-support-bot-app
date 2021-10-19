using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentTypeGetFlowOut
{
    public IncidentTypeGetFlowOut(int caseTypeCode)
        =>
        CaseTypeCode = caseTypeCode;

    public int CaseTypeCode { get; }
}
