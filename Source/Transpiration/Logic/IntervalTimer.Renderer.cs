using System;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace Transpiration
{
    public partial class IntervalTimer
    {
        public void Update(GraphicsLibrary gl)
        {
            long seconds = (DateTime.Now - intervalStart).Ticks / 10000000;
            if (itState == IntervalTimerState.Stop) { seconds = 0; }

            gl.DrawText(0, 0, "Active:", color: Color.White);
            gl.DrawText(gl.Width, 20, GetActiveTime(seconds),
                color: Color.LawnGreen,
                scaleFactor: GraphicsLibrary.ScaleFactor.X3,
                alignment: GraphicsLibrary.TextAlignment.Right);

            gl.DrawText(0, 80, "Rest:", color: Color.White);
            gl.DrawText(gl.Width, 100, GetRestTime(seconds),
                color: Color.Red,
                scaleFactor: GraphicsLibrary.ScaleFactor.X3,
                alignment: GraphicsLibrary.TextAlignment.Right);

            gl.DrawText(0, 160, $"Total (Round {GetInterval(seconds)}):", color: Color.White);
            gl.DrawText(gl.Width, 180, GetTotalTime(seconds),
                color: Color.RoyalBlue,
                scaleFactor: GraphicsLibrary.ScaleFactor.X3,
                alignment: GraphicsLibrary.TextAlignment.Right);
        }
    }
}
