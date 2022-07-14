using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;
using System.Globalization;

namespace Transpiration;

public partial class Clock : IMode
{
    public int RenderDelay => 500;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
    {
        if(showCalendar)
        {
            DrawCalendar(gl);
            return;
        }

        var time = DateTime.Now.ToString("h:mm", CultureInfo.InvariantCulture);

        gl.DrawText(0, 40, time,
            ScaleFactor.X3, TextAlignment.Left);

        gl.DrawText(36*time.Length, 56, DateTime.Now.ToString("tt", CultureInfo.InvariantCulture),
            ScaleFactor.X2, TextAlignment.Left);

        gl.DrawText(10, 110, DateTime.Now.ToString("ddd, MMM d", CultureInfo.InvariantCulture),
            ScaleFactor.X1, TextAlignment.Left);

        var tempString = $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")}°C";

        gl.DrawText(10, 140, tempString,
            ScaleFactor.X1, TextAlignment.Left);
    }
}