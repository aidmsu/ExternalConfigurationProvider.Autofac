#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OpenCover"
#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov

var configuration = Argument("configuration", "Release");
var version = Argument<string>("buildVersion", null);
var target = Argument("target", "Default");

Task("Default").IsDependentOn("Pack");

Task("Clean").Does(()=> 
{
    CleanDirectory("./build");
    StartProcess("dotnet", "clean -c:" + configuration);
});

Task("Restore").Does(()=> 
{
    DotNetCoreRestore();
});

Task("SetAppVeyorVersion").WithCriteria(AppVeyor.IsRunningOnAppVeyor).Does(() => 
{
    version = AppVeyor.Environment.Build.Version;

    if (AppVeyor.Environment.Repository.Tag.IsTag)
    {
        var tagName = AppVeyor.Environment.Repository.Tag.Name;
        if(tagName.StartsWith("v"))
        {
            version = tagName.Substring(1);
        }

        AppVeyor.UpdateBuildVersion(version);
    }
});

Task("Build")
    .IsDependentOn("SetAppVeyorVersion")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(()=> 
{
    var buildSettings =  new DotNetCoreBuildSettings { Configuration = configuration };
    if(!string.IsNullOrEmpty(version)) buildSettings.ArgumentCustomization = args => args.Append("/p:Version=" + version);

    DotNetCoreBuild("src/ExternalConfiguration.Autofac/ExternalConfiguration.Autofac.csproj", buildSettings);
});

Task("Pack").IsDependentOn("Build").Does(()=> 
{
    CreateDirectory("build");
    
    CopyFiles(GetFiles("./src/ExternalConfiguration.Autofac/bin/**/*.nupkg"), "build");
    Zip("./src/ExternalConfiguration.Autofac/bin/" + configuration, "build/ExternalConfigurationPovider.Autofac-" + version +".zip");
});

RunTarget(target);