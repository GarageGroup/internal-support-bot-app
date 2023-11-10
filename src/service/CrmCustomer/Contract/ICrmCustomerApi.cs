using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmCustomerApi
{
    ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetGetFailureCode>>> SearchAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken);

    ValueTask<Result<LastCustomerSetGetOut, Failure<CustomerSetGetFailureCode>>> GetLastAsync(
        LastCustomerSetGetIn input, CancellationToken cancellationToken);
}