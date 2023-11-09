using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmCustomerApi
{
    ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> SearchAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken);
}