using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration;

public partial class Timer : IMode
{
    public int RenderDelay => 500;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
    {

    }
}

