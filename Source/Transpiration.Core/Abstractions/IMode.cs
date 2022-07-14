using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration;

public interface IMode
{
    public void Update(MicroGraphics gl,
        ITemperatureSensor temperatureSensor = null);

    public int RenderDelay { get; }
}
