using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;

namespace Transpiration;

public partial class Timer : IMode
{
    IFont timerFont = new Font4x6();

    public int RenderDelay => 500;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
    {
        gl.Clear();
     
        //draw current timer time
        gl.CurrentFont = timerFont;

        if (timerState == StopWatchState.Start)
        {
            gl.DrawText(0, 1, GeTimerTime((DateTime.Now - timerStart).Ticks + timerOffset));
        }
        else
        {
            gl.DrawText(0, 1, GeTimerTime(timerOffset));
        }

        //draw time
        gl.DrawText(0, 9, $"{DateTime.Now.ToString("h:mm:tt")}");

        //draw temperature
        try
        {
            gl.DrawText(0, 18, $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")} C");
        }
        catch
        {

        }

        gl.Show();
    }
}