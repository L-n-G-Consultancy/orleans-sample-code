using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.IO;
using Microsoft.Extensions.Options;


namespace IOT.SiloHost
{
    public static class ConsoleAppConfigurator
    {
        public static (string env, IConfigurationRoot configurationRoot, OrleansConfig orleansConfig1) BootstrapConfigurationRoot()
        {
            var env = GetEnvironmentName();
            var tempConfigBuilder = new ConfigurationBuilder();

            tempConfigBuilder
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: false);

            var configurationRoot = tempConfigBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<OrleansConfig>(configurationRoot.GetSection(nameof(OrleansConfig)));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var orleansConfig = serviceProvider.GetService<IOptions<OrleansConfig>>().Value;
            return (env, configurationRoot, orleansConfig);
        }

        public static string GetEnvironmentName()
        {
            var env = "dev";// Environment.GetEnvironmentVariable(("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
            {
                throw new Exception("ASPNETCORE_ENVIRONMENT env variable not set.");
            }

            return env;
        }
        public class OrleansConfig
        {
            /// <summary>
            /// The IP addresses that will be utilized in the cluster.
            /// First IP address is the primary.
            /// </summary>
            public string[] NodeIpAddresses { get; set; }
            /// <summary>
            /// The port used for Client to Server communication.
            /// </summary>
            public int GatewayPort { get; set; }
            /// <summary>
            /// The port for Silo to Silo communication
            /// </summary>
            public int SiloPort { get; set; }
        }
    }
}
