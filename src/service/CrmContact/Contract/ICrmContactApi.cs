using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmContactApi
{
    ValueTask<Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>> SearchAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken);
}