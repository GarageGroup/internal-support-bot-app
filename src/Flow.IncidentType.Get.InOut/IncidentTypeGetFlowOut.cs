using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentTypeGetFlowOut
{
    public IncidentTypeGetFlowOut(int caseTypeCode, string caseTypeTitle)
    {
        CaseTypeCode = caseTypeCode;
        CaseTypeTitle = caseTypeTitle;
    }

    public int CaseTypeCode { get; }

    public string CaseTypeTitle {  get; }
}
