using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

internal sealed record class CaseTypeValue
{
    public CaseTypeValue(int code, [AllowNull] string name)
    {
        Code = code;
        Name = name.OrEmpty();
    }

    public int Code { get; }

    public string Name { get; }
}