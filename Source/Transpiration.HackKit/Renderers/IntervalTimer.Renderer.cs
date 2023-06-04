using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;

namespace Transpiration
{
    public partial class IntervalTimer : IMode
    {
        public int RenderDelay => 500;

        public void Update(MicroGraphics gl, ITemperatureSensor sensor = null)
        {
            long seconds = (DateTime.Now - intervalStart).Ticks / 10000000;
            if (itState == IntervalTimerState.Stop) { seconds = 0; }

            gl.DrawText(0, 0, "Active:", color: Color.White);
            gl.DrawText(gl.Width, 20, GetActiveTime(seconds),
                color: Color.LawnGreen,
                scaleFactor: ScaleFactor.X3,
                alignmentH: HorizontalAlignment.Right);

            gl.DrawText(0, 80, "Rest:", color: Color.White);
            gl.DrawText(gl.Width, 100, GetRestTime(seconds),
                color: Color.Red,
                scaleFactor: ScaleFactor.X3,
                alignmentH: HorizontalAlignment.Right);

            gl.DrawText(0, 160, $"Total (Round {GetInterval(seconds)}):", color: Color.White);
            gl.DrawText(gl.Width, 180, GetTotalTime(seconds),
                color: Color.RoyalBlue,
                scaleFactor: ScaleFactor.X3,
                alignmentH: HorizontalAlignment.Right);
        }
    }
}
