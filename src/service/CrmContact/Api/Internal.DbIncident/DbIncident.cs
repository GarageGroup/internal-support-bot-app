using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

[DbEntity("incident", AliasName)]
internal sealed partial record class DbIncident : IDbEntity<DbIncident>
{
    private const string All = "QueryAll";

    private const string AliasName = "i";
}