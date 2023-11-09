using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmIncidentApi
{
    ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> CreateAsync(
        IncidentCreateIn input, CancellationToken cancellationToken);
}