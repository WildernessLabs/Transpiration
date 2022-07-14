using System;
using System.Linq;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration
{
	public partial class Stopwatch : IMode
	{
        public int RenderDelay => 10;

        public void Update(MicroGraphics gl, ITemperatureSensor sensor = null)
        {
            if (swState == StopWatchState.Start)
            {
                gl.DrawText(0, 0, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks + stopwatchOffset),
                        scaleFactor: ScaleFactor.X2);
            }
            else
            {
                gl.DrawText(0, 0, GetStopwatchTime(stopwatchOffset),
                        scaleFactor: ScaleFactor.X2);
            }

            if (splits.Count > 0)
            {   //current lap
                if (swState == StopWatchState.Start)
                {
                    gl.DrawText(0, 40, GetStopwatchTime((DateTime.Now - stopWatchStart).Ticks),
                        scaleFactor: ScaleFactor.X2);
                }
                else
                {
                    gl.DrawText(0, 40, GetStopwatchTime(stopwatchOffset - splits.LastOrDefault()),
                        scaleFactor: ScaleFactor.X2);
                }

                //last split
                gl.DrawText(0, 80, GetStopwatchTime(splits.LastOrDefault()),
                        scaleFactor: ScaleFactor.X2);
            }
        }
	}
}