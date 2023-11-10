using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmOwnerApi
{
    ValueTask<Result<OwnerSetSearchOut, Failure<OwnerSetGetFailureCode>>> SearchAsync(
        OwnerSetSearchIn input, CancellationToken cancellationToken);
}