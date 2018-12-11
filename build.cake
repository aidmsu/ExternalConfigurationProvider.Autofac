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

    DotNetCoreBuild("src/ExternalConfigurationProvider.Autofac/ExternalConfigurationProvider.Autofac.csproj", buildSettings);
});


Task("Test").IsDependentOn("Build").Does(() =>
{
    DotNetCoreTest("./tests/ExternalConfigurationProvider.Autofac.Tests/ExternalConfigurationProvider.Autofac.Tests.csproj", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/p:BuildProjectReferences=false")
    });
});

Task("TestCoverage").IsDependentOn("Test").Does(() => 
{
    OpenCover(
        tool => { tool.XUnit2("tests/ExternalConfigurationProvider.Autofac.Tests/bin/" + configuration + "/**/ExternalConfigurationProvider.Autofac.Tests.dll", new XUnit2Settings { ShadowCopy = false }); },
        new FilePath("coverage.xml"),
        new OpenCoverSettings()
            .WithFilter("+[ExternalConfigurationProvider.Autofac]*")
            .WithFilter("-[ExternalConfigurationProvider.Autofac.Tests]*"));
});

Task("Pack").IsDependentOn("TestCoverage").Does(()=> 
{
    CreateDirectory("build");
    
    CopyFiles(GetFiles("./src/ExternalConfigurationProvider.Autofac/bin/**/*.nupkg"), "build");
    Zip("./src/ExternalConfigurationProvider.Autofac/bin/" + configuration, "build/ExternalConfigurationPovider.Autofac-" + version +".zip");
});

RunTarget(target);