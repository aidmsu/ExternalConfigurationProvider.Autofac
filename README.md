# ExternalConfigurationProvider.Autofac

[![NuGet](https://img.shields.io/nuget/v/ExternalConfigurationProvider.Autofac.svg)](https://www.nuget.org/packages/ExternalConfigurationProvider.Autofac)
[![AppVeyor](https://img.shields.io/appveyor/ci/aidmsu/ExternalConfigurationProvider-Autofac/master.svg?label=appveyor)](https://ci.appveyor.com/project/aidmsu/ExternalConfigurationProvider-Autofac/branch/master)
[![Codecov branch](https://img.shields.io/codecov/c/github/aidmsu/ExternalConfigurationProvider.Autofac/master.svg)](https://codecov.io/gh/aidmsu/ExternalConfigurationProvider.Autofac)

Autofac integration for [ExternalConfigurationProvider](https://github.com/aidmsu/ExternalConfigurationProvider). Provides registration extensions, allowing you to use Autofac container to resolve `IExternalConfigurationProvider`.

## Usage

Register `IExternalConfigurationProvider` in Autofac:

```csharp
var environment = ... ; // You can get value from appsettings.json or app.config.

builder.RegisterConsulConfigurationProvider(environment, config =>
{
    config.Url = Properties.Settings.Default.ConsulUrl;
    config.Token = Properties.Settings.Default.ConsulToken;
});
```

Use `IExternalConfigurationProvider` to get service settings:

```csharp
builder
    .Register(c =>
    {
        var provider = c.Resolve<IExternalConfigurationProvider>();
        var settings = provider.GetServiceSettingsAsync("redis").Result;

        return new RedisClient(settings["url"], settings["url"]);
    })
    .As<IRedisClient>()
```

Or

```csharp
builder
    .RegisterType<RedisClient>()
    .As<IRedisClient>();

...

public class RedisClient
{
    public Repository(IExternalConfigurationProvider externalConfigurationProvider)
    {
	    var redisConfig = externalConfigurationProvider.GetServiceSettingsAsync("redis").Result;
	    _url = redisConfig["url"];
	    _password = redisConfig["password"];
    }
}
```
