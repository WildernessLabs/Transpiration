using System;
using System.Linq;
using Meadow.Foundation.Graphics;

namespace Transpiration
{
	public partial class Stopwatch
	{
        FontBase fontDate;

		public void Update(GraphicsLibrary gl)
        {
            if(fontDate == null)
            {
                fontDate = new Font4x6();
            }

            gl.Clear();
            gl.CurrentFont = fontDate;

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
}
