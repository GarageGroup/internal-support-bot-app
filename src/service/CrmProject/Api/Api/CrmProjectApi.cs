using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmProjectApi(ISqlQueryEntitySetSupplier sqlApi) : ICrmProjectApi;