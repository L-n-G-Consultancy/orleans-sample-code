using IOT.GrainClasses;
using IOT.GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.Storage;
using System;


using System.Collections.Generic;
using System.Net;
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
            var endPoints = new List<IPEndPoint>();

            var ipAddress = IPAddress.Loopback;
            var port = 30000;

            endPoints.Add(new IPEndPoint(ipAddress, port));

            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "SimpleSample";
                    options.ServiceId = "SimpleSample";
                })
               
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IDeviceGrain).Assembly).WithReferences())
                .UseStaticClustering(endPoints.ToArray())
                .Build();

            await client.Connect();
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            var strGuid = Guid.NewGuid();

          
            // example of calling grains from the initialized client
            var grain = client.GetGrain<IDeviceGrain>(strGuid);
          //  var grain = IGrainFactory.GetGrain<IDeviceGrain>(strGuid);
             while (true)
            {
                var temp = Console.ReadLine();

                int.TryParse(temp, out int tempData);
                await grain.SetTemperature(tempData);
            }
            // Console.WriteLine($"\n\n{response}\n\n");
        }
    }

}
