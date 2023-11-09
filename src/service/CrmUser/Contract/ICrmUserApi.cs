using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmUserApi
{
    ValueTask<Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>> SearchAsync(
        UserSetSearchIn input, CancellationToken cancellationToken);
}