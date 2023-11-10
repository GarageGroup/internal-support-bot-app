using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmContactApi
{
    ValueTask<Result<ContactSetSearchOut, Failure<ContactSetGetFailureCode>>> SearchAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken);

    ValueTask<Result<LastContactSetGetOut, Failure<ContactSetGetFailureCode>>> GetLastAsync(
        LastContactSetGetIn input, CancellationToken cancellationToken);
}