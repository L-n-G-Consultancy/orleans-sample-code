using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IOT.GrainClasses;
using IOT.GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Core;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;

namespace IOT.TestSilo
{
    public class Program
    {

        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();

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

        private static async Task<ISiloHost> StartSilo()
        {
            bool myMemberComesFromInterface = typeof(DeviceGrain).GetInterfaces()
    .SelectMany(i => i.GetMember("SetTemperature")).Any();
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                  .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)

                .AddMemoryGrainStorage("OrleansMemoryProvider")
                 .AddMemoryGrainStorageAsDefault()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                 .UseEnvironment("Dev")
                                   .UseDashboard(options => { })
                //        .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DeviceGrain).Assembly).WithReferences())
                // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IAddressable).Assembly).WithReferences())
                // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ILifecycleParticipant<>).Assembly).WithReferences())
                //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainWithGuidKey).Assembly).WithReferences())
                // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrain).Assembly).WithReferences())
                //             .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainStorage).Assembly).WithReferences())
                //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(MemoryGrainStorage).Assembly).WithReferences())

                //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StorageProvider).Assembly).WithReferences())
                //    .ConfigureApplicationParts(parts => parts.AddFeatureProvide(();
                .ConfigureLogging(logging => logging.AddConsole());


            var host = builder.Build();

            await host.StartAsync();
            return host;
        }
    }
}
