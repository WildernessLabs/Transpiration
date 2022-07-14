using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;
using System.Globalization;

namespace Transpiration;

public partial class Timer : IMode
{
    public int RenderDelay => 500;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
    {
        gl.PenColor = Color.White;

        string text;
        if (timerState == StopWatchState.Start)
        {
            text = GeTimerTime((DateTime.Now - timerStart).Ticks + timerOffset);
        }
        else
        {
            text = GeTimerTime(timerOffset);
        }

        gl.DrawText(120, 60, text,
                alignment: TextAlignment.Center,
                scaleFactor: ScaleFactor.X3);

        gl.PenColor = Color.Orange;

        text = DateTime.Now.ToString("h:mmtt", CultureInfo.InvariantCulture);
        gl.DrawText(240, 160, text,
            scaleFactor: ScaleFactor.X2,
            alignment: TextAlignment.Right);

        text = $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")}°C";

        gl.DrawText(240, 200, text,
            scaleFactor: ScaleFactor.X2,
            alignment: TextAlignment.Right);
    }
}