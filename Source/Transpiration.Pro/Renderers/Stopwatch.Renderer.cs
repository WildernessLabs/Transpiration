using System;
using System.Linq;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration;

public partial class Stopwatch : IMode
{
    IFont fontStopwatch;

    public int RenderDelay => 10;

	public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor = null)
    {
        if(fontStopwatch == null)
        {
            fontStopwatch = new Font4x6();
        }

        gl.Clear();
        gl.CurrentFont = fontStopwatch;

        if (swState == StopWatchState.Start)
        {
            gl.DrawText(0, 0, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks + stopwatchOffset));
        }
        else
        {
            gl.DrawText(0, 0, GetStopwatchTime(stopwatchOffset));
        }

        if (splits.Count > 0)
        {
            //current lap
            if (swState == StopWatchState.Start)
            {
                gl.DrawText(0, 8, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks));
            }
            else
            {
                gl.DrawText(0, 8, GetStopwatchTime(stopwatchOffset - splits.LastOrDefault()));
            }

            //last split
            gl.DrawText(0, 16, GetStopwatchTime(splits.LastOrDefault()));

            //   gl.CurrentFont = fontClock;
            //    gl.DrawText(0, 22, $"Lap {splits.Count + 1}");
            //    gl.DrawText(0, 44, $"Lap {splits.Count} split"); 
        }

        gl.Show();
    }
}