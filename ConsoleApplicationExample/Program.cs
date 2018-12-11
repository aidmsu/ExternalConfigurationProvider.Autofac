using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using ExternalConfiguration;
using ExternalConfiguration.Autofac;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplicationExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var containerBuilder = new ContainerBuilder();

            var consulUrl = configuration.GetSection("Consul:Url").Value;
            var consulToken = configuration.GetSection("Consul:Token").Value;
            var environment = configuration.GetSection("Environment").Value;

            // Previously you need to add to

            containerBuilder.RegisterConsulConfigurationProvider(environment, config =>
            {
                config.Url = consulUrl;
                config.Token = consulToken;
            });

            containerBuilder
                .Register(c =>
                {
                    var configurationProvider = c.Resolve<IExternalConfigurationProvider>();

                    var clientKey = configuration.GetSection("Consul:ClientKey").Value;
                    var clientSettings = configurationProvider.GetServiceSettingsAsync(clientKey).Result;

                    string url = null;
                    clientSettings?.TryGetValue("url", out url);

                    if (url == null) Console.WriteLine($"Consul KV storage doesn't contain value for '{clientKey}' key.");

                    return new Client(url);
                })
                .As<IClient>();

            var serviceProvder = containerBuilder.Build();

            var client = serviceProvder.Resolve<IClient>();
            await client.SendRequestAsync();
            Console.ReadLine();
        }
    }
}
