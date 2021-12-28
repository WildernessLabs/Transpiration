using System;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration;

public partial class IntervalTimer : IMode
{
    IFont fontClock;
    IFont fontDate;

    public int RenderDelay => 500;

    public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor = null)
    {
        if(fontClock == null)
        {
            fontDate = new Font4x6();
            fontClock = new Font6x8();
        }

        gl.Clear();

        long seconds = (DateTime.Now - intervalStart).Ticks / 10000000;
        if (itState == IntervalTimerState.Stop) { seconds = 0; }

        gl.CurrentFont = fontClock;

        gl.DrawText(gl.Width + 1, 0, GetActiveTime(seconds), alignment: TextAlignment.Right);
        gl.DrawText(gl.Width + 1, 8, GetRestTime(seconds), alignment: TextAlignment.Right);

        gl.CurrentFont = fontDate;
        gl.DrawText(0, 18, $"{GetInterval(seconds)}");
        gl.DrawText(gl.Width + 1, 18, GetTotalTime(seconds), alignment: TextAlignment.Right);

        gl.Show();
    }
}