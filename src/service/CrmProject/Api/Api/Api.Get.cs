using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmProjectApi
{
    public ValueTask<Result<ProjectSetGetOut, Failure<Unit>>> GetAsync(
        ProjectSetGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static @in => DbProject.QueryAll with
            {
                Filter = new DbCombinedFilter(DbLogicalOperator.And)
                {
                    Filters =
                    [
                        DbProject.IsActiveFilter,
                        DbProject.BuildClientIdFilter(@in.CustomerId)
                    ]
                },
            })
        .PipeValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbProject>)
        .MapSuccess(
            static success => new ProjectSetGetOut
            {
                Projects = success.Map(MapProject)
            });

    private static ProjectItemOut MapProject(DbProject dbProject)
        =>
        new(
            id: dbProject.ProjectId,
            name: dbProject.ProjectName);
}
