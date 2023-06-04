using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;

namespace Transpiration;

public partial class Timer : IMode
{
    IFont timerFont = new Font4x6();

    public int RenderDelay => 100;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
    {
        gl.Clear();
     
        //draw current timer time
        if (timerState == StopWatchState.Start)
        {
            gl.DrawText(33, 1, GeTimerTime((DateTime.Now - timerStart).Ticks + timerOffset), alignmentH: HorizontalAlignment.Right);
        }
        else
        {
            gl.DrawText(33, 1, GeTimerTime(timerOffset), alignmentH: HorizontalAlignment.Right);
        }

        //draw time
        gl.DrawText(33, 9, $"{DateTime.Now.ToString("h:mm:tt")}", alignmentH: HorizontalAlignment.Right);

        //draw temperature
        try
        {
            gl.DrawText(33, 18, $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")} C", alignmentH: HorizontalAlignment.Right);
        }
        catch
        {

        }

        gl.Show();
    }
}