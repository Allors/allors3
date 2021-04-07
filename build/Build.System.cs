using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    private Target AdaptersGenerate => _ => _
        .After(this.Clean)
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(this.Paths.SystemRepositoryGenerate)
                .SetApplicationArguments(
                    $"{this.Paths.SystemAdaptersRepositoryDomainRepository} {this.Paths.SystemRepositoryTemplatesMetaCs} {this.Paths.SystemAdaptersMetaGenerated}"));
            DotNetRun(s => s
                .SetProcessWorkingDirectory(this.Paths.SystemAdapters)
                .SetProjectFile(this.Paths.SystemAdaptersGenerate));
        });

    private Target AdaptersTestMemory => _ => _
        .DependsOn(this.AdaptersGenerate)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(this.Paths.SystemAdaptersStaticTests)
            .SetFilter("FullyQualifiedName~Allors.Database.Adapters.Memory")
            .SetLogger("trx;LogFileName=AdaptersMemory.trx")
            .SetResultsDirectory(this.Paths.ArtifactsTests)));

    private Target AdaptersTestSqlClient => _ => _
        .DependsOn(this.AdaptersGenerate)
        .Executes(() =>
        {
            using (new SqlServer())
            {
                DotNetTest(s => s
                    .SetProjectFile(this.Paths.SystemAdaptersStaticTests)
                    .SetFilter("FullyQualifiedName~Allors.Database.Adapters.SqlClient")
                    .SetLogger("trx;LogFileName=AdaptersSqlClient.trx")
                    .SetResultsDirectory(this.Paths.ArtifactsTests));
            }
        });

    private Target AdaptersTestNpgsql => _ => _
        .DependsOn(this.AdaptersGenerate)
        .Executes(() =>
        {
            using (new Postgres())
            {
                DotNetTest(s => s
                    .SetProjectFile(this.Paths.SystemAdaptersStaticTests)
                    .SetFilter("FullyQualifiedName~Allors.Database.Adapters.Npgsql")
                    .SetLogger("trx;LogFileName=AdaptersNpgsql.trx")
                    .SetResultsDirectory(this.Paths.ArtifactsTests));
            }
        });

    private Target Adapters => _ => _
        .DependsOn(this.Clean)
        .DependsOn(this.AdaptersTestMemory)
        .DependsOn(this.AdaptersTestSqlClient)
        .DependsOn(this.AdaptersTestNpgsql);
}
