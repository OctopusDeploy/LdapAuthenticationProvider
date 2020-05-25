using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.WindowsLatest,
    OnPullRequestBranches = new[] { "master" },
    OnPushBranches = new[] { "master" },
    InvokedTargets = new[] { nameof(Publish), nameof(Pack) })]
internal class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] private readonly Solution Solution;
    [GitVersion] private readonly GitVersion GitVersion;

    private AbsolutePath SourceDirectory => RootDirectory / "source";
    private AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    private AbsolutePath PublishDirectory => ArtifactsDirectory / "publish";
    private AbsolutePath PackDirectory => ArtifactsDirectory / "pack";

    Project ExtensionProject => Solution.GetProject("Server");

    private Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    private Target Restore => _ => _
     .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    private Target Publish => _ => _
        .DependsOn(Restore)
        .Produces(PublishDirectory)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(ExtensionProject)
                .SetOutput(PublishDirectory)
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.FullSemVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            Logger.Info("Deleting all files except primary output and dependencies, as this will be loaded as an extension.");

            PublishDirectory.GlobFiles("*.*")
                .Except(new[]
                {
                    PublishDirectory / $"{ExtensionProject.GetProperty("AssemblyName")}.dll",
                    PublishDirectory / "Novell.Directory.Ldap.NETStandard.dll"
                }).ForEach(DeleteFile);
        });

    private Target Pack => _ => _
        .DependsOn(Restore)
        .Produces(PackDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(ExtensionProject)
                .SetOutputDirectory(PackDirectory)
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.FullSemVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });
}