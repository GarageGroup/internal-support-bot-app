using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

[DbEntity("contact", AliasName)]
internal sealed partial record class DbContact : IDbEntity<DbContact>
{
    private const string All = "QueryAll";

    private const string AliasName = "c";

    private const string IncidentAlias = "i";

    private const string IdFieldName = $"{AliasName}.contactid";

    private const string LastIncidentDateQuery
        =
        $"SELECT MAX({IncidentAlias}.createdon) FROM incident {IncidentAlias} WHERE {IncidentAlias}.primarycontactid = {IdFieldName}";

    private const string LastIncidentDateParameter = "LastIncidentDate";

    private const string LastCurrentUserIncidentDateParameter = "LastCurrentUserIncidentDate";
}