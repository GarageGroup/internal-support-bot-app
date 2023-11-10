using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

[DbEntity("incident", AliasName)]
[DbJoin(DbJoinType.Inner, "systemuser", UserAlias, $"{UserAlias}.systemuserid = {OwnerIdFieldName}")]
internal sealed partial record class DbIncidentOwner : IDbEntity<DbIncidentOwner>
{
    private const string All = "QueryAll";

    private const string AliasName = "i";

    private const string UserAlias = "u";

    private const string OwnerIdFieldName = $"{AliasName}.ownerid";

    private const string MaxCreatedOnParameter = "MaxCreatedOn";
}