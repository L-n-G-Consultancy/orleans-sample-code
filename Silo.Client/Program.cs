using IOT.GrainClasses;
using IOT.GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Threading.Tasks;

namespace Silo.Client
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()

                  .Configure<ClusterOptions>(options =>
                  {
                      options.ClusterId = "dev";
                      options.ServiceId = "OrleansBasics";
                  })
               //   .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IDeviceGrain).Assembly).WithReferences())
                  //  .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IAddressable).Assembly).WithReferences())
                  // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ILifecycleParticipant<>).Assembly).WithReferences())
                  //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainWithGuidKey).Assembly).WithReferences())
                  // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrain).Assembly).WithReferences())
                  //   .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainStorage).Assembly).WithReferences())
                  //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(MemoryGrainStorage).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
          

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            var strGuid = Guid.NewGuid();

          //  var temp = Console.ReadLine();
            
         //   int.TryParse(temp, out int tempData);
            // example of calling grains from the initialized client
            var grain = client.GetGrain<IDeviceGrain>(strGuid);
           // var grain = IGrainFactory.GetGrain<IDeviceGrain>(strGuid);
            // while (true)
            {

                await grain.SetTemperature(1);
            }
            // Console.WriteLine($"\n\n{response}\n\n");
        }
    }

}
