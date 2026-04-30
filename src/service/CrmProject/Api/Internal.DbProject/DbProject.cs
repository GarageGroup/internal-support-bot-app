using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

[DbEntity("gg_project", AliasName)]
internal sealed partial record class DbProject : IDbEntity<DbProject>
{
    private const string All = "QueryAll";

    private const string AliasName = "p";
}