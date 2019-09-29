using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AbcVersionTool;
using Helpers;
using Helpers.Azure;
using Helpers.Syrup;
using Models;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[Serializable]
class Build : NukeBuild
{
    [Parameter("Build counter from outside environment")] readonly int BuildCounter;

    readonly DateTime BuildDate = DateTime.UtcNow;

    [GitRepository] readonly GitRepository GitRepository;

    [Solution("src/LukeSearch.sln")] readonly Solution Solution;

    bool IsTeamCity = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")) == false;
    readonly bool IsAzureDevOps = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_NAME")) == false;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    string Configuration => IsTeamCity ? "Release" : "Debug";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath TmpBuild => TemporaryDirectory / "build";
    AbsolutePath ToolsDir => RootDirectory / "tools";
    AbsolutePath LibzPath => ToolsDir / "LibZ.Tool" / "tools" / "libz.exe";
    AbsolutePath NugetPath => ToolsDir / "nuget.exe";
    AbsolutePath SourceDir => RootDirectory / "src";
    Project LukeSearchProject => Solution.GetProject("ChromiumWebBrowser").NotNull();
  

    ProductInfo ProductInfo => new ProductInfo
    {
        Company = "Deneblab",
        Copyright = $"Deneblab (c) {DateTime.UtcNow.Year}"
    };


    List<ProjectDefinition> Projects => new List<ProjectDefinition>
    {
        new ProjectDefinition
        {
            Name = "ChromiumWebBrowser",
            Dir = "ChromiumWebBrowser",
            Exe = "ChromiumWebBrowser.exe",
            DstExe = "ChromiumWebBrowser.exe",
            AzureContainerName = "application-chromium-webbrowser",
            Project = LukeSearchProject
        },
      
    };

    AbcVersion Version => AbcVersionFactory.Create(BuildCounter, BuildDate);

    Target Information => _ => _
        .Executes(() =>
        {
            var b = Version;
            Logger.Info($"Host: '{Host}'");
            Logger.Info($"Version: '{b.SemVersion}'");
            Logger.Info($"Date: '{b.DateTime:s}Z'");
            Logger.Info($"FullVersion: '{b.InformationalVersion}'");
        });


    Target ConfigureAzureDevOps => _ => _
        .DependsOn(Information)
        .OnlyWhenStatic(() => IsAzureDevOps)
        .Executes(() =>
        {
            Logger.Normal($"Set version to AzureDevOps: {Version.SemVersion}");
            // https://github.com/microsoft/azure-pipelines-tasks/blob/master/docs/authoring/commands.md
            Logger.Normal($"##vso[build.updatebuildnumber]{Version.SemVersion}");
        });


    Target Configure => _ => _
        .DependsOn(ConfigureAzureDevOps);





    Target CheckTools => _ => _
        .DependsOn(Configure)
        .Executes(() =>
        {
            Downloader.DownloadIfNotExists("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", NugetPath,
                "Nuget");
        });

    Target Clean => _ => _
        .DependsOn(CheckTools)
        .Executes(() =>
        {
            EnsureExistingDirectory(TmpBuild);
            GlobDirectories(TmpBuild, "**/*")
                .ToList()
                .ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            using (var process = ProcessTasks.StartProcess(
                NugetPath,
                $"restore  {Solution.Path}",
                SourceDir))
            {
                process.AssertWaitForExit();
                ControlFlow.AssertWarn(process.ExitCode == 0,
                    "Nuget restore report generation process exited with some errors.");
            }
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>

        {
            foreach (var p in Projects)
            {
                var buildOut = TmpBuild / CommonDir.Build / p.Dir;
                var projectFile = p.Project.Path;
                var projectDir = Path.GetDirectoryName(projectFile);
                EnsureExistingDirectory(buildOut);
                Logger.Info($"Build; Project file: {projectFile}");
                Logger.Info($"Build; Project dir: {projectDir}");
                Logger.Info($"Build; Out dir: {buildOut}");
                Logger.Info($"Build; Configuration: {Configuration}");


                try
                {
                    AssemblyTools.Patch(projectDir, Version, p, ProductInfo);

                    MSBuild(s => s
                        .SetProjectFile(projectFile)
                        .SetOutDir(buildOut)
                        .SetVerbosity(MSBuildVerbosity.Quiet)
                        .SetConfiguration(Configuration)
                        .SetTargetPlatform(MSBuildTargetPlatform.x64)
                        .SetMaxCpuCount(Environment.ProcessorCount)
                        .SetNodeReuse(IsLocalBuild));
                }
                finally
                {
                    AssemblyTools.RollbackOriginalFiles(projectDir);

                }
            }
        });


    Target Marge => _ => _
        .DependsOn(Compile)
        .Executes(() =>

        {
            foreach (var p in Projects)
            {
                var buildOut = TmpBuild / CommonDir.Build / p.Dir;
                var margeOut = TmpBuild / CommonDir.Merge / p.Dir;

                EnsureExistingDirectory(margeOut);
                CopyDirectoryRecursively(buildOut, margeOut, DirectoryExistsPolicy.Merge);

                using (var process = ProcessTasks.StartProcess(
                    LibzPath,
                    $"inject-dll --assembly {p.Exe} --include *.dll --move",
                    margeOut))
                {
                    process.AssertWaitForExit();
                    ControlFlow.AssertWarn(process.ExitCode == 0,
                        "Libz report generation process exited with some errors.");
                }
            }
        });

    Target CopyToReady => _ => _
        .DependsOn(Marge)
        .Executes(() =>
        {
            var readOut = TmpBuild / CommonDir.Ready;
            var srcBuild = SourceDir / "build";
            EnsureExistingDirectory(readOut);

            foreach (var p in Projects)
            {
                var margeOut = TmpBuild / CommonDir.Merge / p.Dir;

                var configsDir = SourceDir / "build" / "configs";
                EnsureExistingDirectory(readOut);
                CopyFile(margeOut / p.Exe,
                    readOut / p.Exe,
                    FileExistsPolicy.Overwrite);
            }


            var nlog = srcBuild / "nlog" / "main";
            CopyDirectoryRecursively(nlog, readOut, DirectoryExistsPolicy.Merge);

        });

    Target Nuget => _ => _
        .DependsOn(CopyToReady)
        .Executes(() =>

        {
            var nugetTmpOut = TmpBuild / "nugetTmp";
            var srcBuild = SourceDir / "build";
            var tmpReady = TmpBuild / CommonDir.Ready;
            var nugetDir = TmpBuild / "nuget";
            var tmpMain = nugetTmpOut / "main";

            EnsureExistingDirectory(nugetTmpOut);

            // copy main files
            CopyDirectoryRecursively(tmpReady, tmpMain, DirectoryExistsPolicy.Merge);

            // syrup scripts
            var src1 = srcBuild / "syrup";
            var dst1 = nugetTmpOut / "_syrup";
            EnsureExistingDirectory(dst1);
            CopyDirectoryRecursively(src1, dst1, DirectoryExistsPolicy.Merge);

            // nuget definition
            var srcNugetFile = srcBuild / "nuget" / "nuget.nuspec";
            var dstNugetFile = nugetTmpOut / "LukeSearch.nuspec";
            var text = File.ReadAllText(srcNugetFile);
            var r = text.Replace("{Version}", Version.NugetVersion);
            File.WriteAllText(dstNugetFile, r, Encoding.UTF8);

            // create nuget
            using (var process = ProcessTasks.StartProcess(
                NugetPath,
                $"pack {dstNugetFile} -OutputDirectory {nugetDir} -NoPackageAnalysis",
                nugetTmpOut))
            {
                process.AssertWaitForExit();
                ControlFlow.AssertWarn(process.ExitCode == 0,
                    "Nuget report generation process exited with some errors.");
            }

            var nugetFiles = GlobFiles(nugetDir, "*.nupkg");

            foreach (var file in nugetFiles) SyrupTools.MakeSyrupFile(file, Version, "LukeSearch");
        });



    Target Publish => _ => _
        .DependsOn(Nuget, PublishLocal, PublishTeamCity, PublishAzureDevOps);

    Target PublishAzureDevOps => _ => _
        .DependsOn(PublishAzureDevOpsArtifacts, PublishAzureDevOpsStorage);

    Target PublishAzureDevOpsArtifacts => _ => _
         .DependsOn(Nuget)
         .OnlyWhenStatic(() => IsAzureDevOps)
         .Executes(() =>
         {
             var p = Projects.FirstOrDefault(x => x.Project == LukeSearchProject);
             if (p == null)
                 throw new KeyNotFoundException($"Project: '{LukeSearchProject}' not found in projects list");
             var tmpReady = TmpBuild / CommonDir.Nuget ;
             EnsureExistingDirectory(tmpReady);
             Logger.Info($"Path: `{tmpReady}`");
             var globFiles = GlobFiles(tmpReady, "*.zip");
             Logger.Info($"Artifact files: {globFiles.Count}");
             globFiles.ForEach(x => Logger.Info($"{x}"));
             var serverPublishArtifact = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");
             CopyDirectoryRecursively(tmpReady, serverPublishArtifact, DirectoryExistsPolicy.Merge);
         });

    Target PublishAzureDevOpsStorage => _ => _
        .DependsOn(PublishAzureDevOpsArtifacts)
        .OnlyWhenStatic(() => IsAzureDevOps)
        .Executes(async () =>
        {
            void LogFiles(string title, List<ReleaseInfo> filesToShow)
            {
                Logger.Info($"{title}: {filesToShow.Count}");
                foreach (var l in filesToShow) Logger.Info($"Name: {l.Name}; Date: {l.ReleaseDate}");
            }

            var p = Projects.FirstOrDefault(x => x.Project == LukeSearchProject);
            if (p == null)
                throw new KeyNotFoundException($"Project: '{LukeSearchProject}' not found in projects list");

            var storageConnectionString = Environment.GetEnvironmentVariable("azureStorageConnectionStringKey1");
            Logger.Info($"Build; storageConnectionString: {storageConnectionString}");
            var serverPublishArtifact = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");
            var files = Directory.GetFiles(serverPublishArtifact).ToList();
            var client = AzureSyrupTools.Create(storageConnectionString, p.AzureContainerName);
            await client.UploadFiles(files);
            var list = await client.GetSyrupFiles();
            var fileToRemove = list.OrderByDescending(x => x.ReleaseDate).Skip(15).ToList();
            LogFiles("Files to remove", fileToRemove);
            await client.RemoveSyrupFiles(fileToRemove);
            var newList = await client.GetSyrupFiles();
            await client.CreateSyrupFilesList(newList);
            LogFiles("Files in container", newList);
        });


    Target PublishLocal => _ => _
        .DependsOn(Nuget)
        .Executes(() =>
        {
        });

    Target PublishTeamCity => _ => _
        .DependsOn(Nuget)
        .OnlyWhenStatic(() => IsTeamCity)
        .Executes(() =>
        {


            var serverPublishDir = @"C:\work\users\AntyPiracy\LukeSearch\syrup";
            var nugetDir = TmpBuild / "nuget";

            EnsureExistingDirectory(serverPublishDir);
            CopyDirectoryRecursively(nugetDir, serverPublishDir, DirectoryExistsPolicy.Merge);
        });

    public static int Main() => Execute<Build>(x => x.Publish);
}