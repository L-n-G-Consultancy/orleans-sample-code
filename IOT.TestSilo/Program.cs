using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IOT.GrainClasses;
using IOT.GrainInterfaces;
using IOT.SiloHost;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Core;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using static IOT.SiloHost.ConsoleAppConfigurator;

namespace IOT.TestSilo
{
    public class Program
    {

        public static int Main(string[] args)
        {
            var (env, configurationRoot, orleansConfig) =
            ConsoleAppConfigurator.BootstrapConfigurationRoot();

            return RunMainAsync(args, env, configurationRoot, orleansConfig).Result;
        }

        private static async Task<int> RunMainAsync(string[] args, string env, IConfigurationRoot configurationRoot, OrleansConfig orleansConfig)
        {
            try
            {
                var host = BuildSilo(env ,orleansConfig);

                await host.StartAsync();

                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static ISiloHost BuildSilo(string env, OrleansConfig orleansConfig)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(orleansConfig.NodeIpAddresses[1].ToString());

            IPEndPoint iPEndPoint = new IPEndPoint(ipaddress.Address, orleansConfig.SiloPort);
            var builder = new SiloHostBuilder()
               // .UseLocalhostClustering()
               .UseDevelopmentClustering(iPEndPoint)
               
                .ConfigureLogging(logging => logging.AddConsole())
                     .UseDashboard(options => { })
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("OrleansMemoryProvider")
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DeviceGrain).Assembly).WithReferences())
                  .UseInMemoryReminderService()
                  .ConfigureEndpoints(ipaddress, orleansConfig.SiloPort, orleansConfig.GatewayPort,false)
                  .UseEnvironment("dev")
                   .Configure<ClusterOptions>(options =>
                   {
                       options.ClusterId = "SimpleSample";
                       options.ServiceId = "SimpleSample";
                   })
            //    .AddMemoryStreams<IDeviceGrainState>("",null)
               ;

            return builder.Build();
        }
    }
}
