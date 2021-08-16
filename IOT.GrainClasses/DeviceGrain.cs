using IOT.GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace IOT.GrainClasses
{

   
    [StorageProvider(ProviderName = "OrleansMemoryProvider")]
    public class DeviceGrain : Grain<DeviceGrainState>, IDeviceGrain
    { 
        public override Task OnActivateAsync()
        {
            var id = this.GetGrainIdentity();
            Console.WriteLine("Activated {0}", id);

            return base.OnActivateAsync();
        }
        public  Task SetTemperature(double value)
        {
            Console.WriteLine("Temperature recorded {0}", value);
            if (State.Lastvaue < 100 && value > 100)
            {
                Console.WriteLine("High Temperature recorded {0}", value);
            }
            if (this.State.Lastvaue != value)
            {
                this.State.Lastvaue = value;
            }

            return Task.CompletedTask;
        }
    }
}
 