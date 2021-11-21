using System;
using System.Linq;
using Meadow.Foundation.Graphics;

namespace Transpiration
{
	public partial class Stopwatch
	{
		public void Update(GraphicsLibrary gl)
        {
            if (swState == StopWatchState.Start)
            {
                gl.DrawText(0, 0, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks + stopwatchOffset),
                        scaleFactor: GraphicsLibrary.ScaleFactor.X2);
            }
            else
            {
                gl.DrawText(0, 0, GetStopwatchTime(stopwatchOffset),
                        scaleFactor: GraphicsLibrary.ScaleFactor.X2);
            }

            if (splits.Count > 0)
            {   //current lap
                if (swState == StopWatchState.Start)
                {
                    gl.DrawText(0, 40, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks),
                        scaleFactor: GraphicsLibrary.ScaleFactor.X2);
                }
                else
                {
                    gl.DrawText(0, 40, GetStopwatchTime(stopwatchOffset - splits.LastOrDefault()),
                        scaleFactor: GraphicsLibrary.ScaleFactor.X2);
                }

                //last split
                gl.DrawText(0, 80, GetStopwatchTime(splits.LastOrDefault()),
                        scaleFactor: GraphicsLibrary.ScaleFactor.X2);
            }
        }
	}
}
