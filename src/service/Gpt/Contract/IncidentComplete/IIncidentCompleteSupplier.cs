using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface IIncidentCompleteSupplier
{
    ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken);
}