using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOT.GrainInterfaces
{
    public interface IDeviceGrainState : IGrainState
    {
        double Lastvaue { get; set; }

    }
    public interface IDeviceGrain:IGrainWithGuidKey
    {
        Task SetTemperature(double value);
    }
}
