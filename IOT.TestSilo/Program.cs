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
                var host = BuildSilo();

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

        private static ISiloHost BuildSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
               
                .ConfigureLogging(logging => logging.AddConsole())
                     .UseDashboard(options => { })
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("OrleansMemoryProvider")
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DeviceGrain).Assembly).WithReferences())
                  .UseInMemoryReminderService()
            //    .AddMemoryStreams<IDeviceGrainState>("",null)
               ;

            return builder.Build();
        }
    }
}
