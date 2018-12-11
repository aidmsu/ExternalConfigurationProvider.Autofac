using System;
using Autofac;

namespace ExternalConfiguration.Autofac
{
    /// <exclude /> 
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Configure Consul configuration provider services.
        /// Service configs will be cached. To avoid caching use overloaded method."/>
        /// </summary>
        public static void RegisterConsulConfigurationProvider(this ContainerBuilder builder, string environment, Action<ConsulConfig> configuration)
        {
            RegisterConsulConfigurationProvider(builder, environment, true, configuration);
        }

        /// <summary>
        /// Configure Consul configuration provider services.
        /// </summary>
        public static void RegisterConsulConfigurationProvider(this ContainerBuilder builder, string environment, bool useCache, Action<ConsulConfig> configuration)
        {
            if (string.IsNullOrEmpty(environment)) throw new ArgumentNullException(nameof(environment));

            var config = new ConsulConfig();
            configuration(config);

            builder.Register(context => new ConsulConfigurationStore(config)).As<IExternalConfigurationStore>();

            builder.Register(context => new ExternalConfiguration.ExternalConfigurationProvider(
                    context.Resolve<IExternalConfigurationStore>(),
                    environment,
                    useCache))
                .As<IExternalConfigurationProvider>()
                .SingleInstance();
        }
    }
}
