using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface ICrmProjectApi
{
    ValueTask<Result<ProjectSetGetOut, Failure<Unit>>> GetAsync(
        ProjectSetGetIn input, CancellationToken cancellationToken);
}