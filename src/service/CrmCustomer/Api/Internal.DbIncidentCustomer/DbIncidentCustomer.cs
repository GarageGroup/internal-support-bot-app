using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

[DbEntity("incident", AliasName)]
[DbJoin(DbJoinType.Inner, "account", AccountAlias, $"{AccountAlias}.accountid = {CustomerIdFieldName}")]
internal sealed partial record class DbIncidentCustomer : IDbEntity<DbIncidentCustomer>
{
    private const string All = "QueryAll";

    private const string AliasName = "i";

    private const string AccountAlias = "a";

    private const string CustomerIdFieldName = $"{AliasName}.customerid";

    private const string CreatedOnFieldName = $"{AliasName}.createdon";

    private const string MaxCreatedOnParameter = "MaxCreatedOn";

    private const string MaxCurrentUserCreatedOnParameter = "MaxCurrentUserCreatedOn";
}