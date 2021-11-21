using System;
using System.Collections.Generic;

namespace Transpiration
{
    public partial class Stopwatch
    {
        enum StopWatchState
        {
            Stop,
            Start,
            Pause
        }

        StopWatchState swState = StopWatchState.Stop;

        DateTime stopWatchStart = DateTime.MinValue;

        List<long> splits = new();
        long stopwatchOffset;
        long lastSplit;

        string GetStopwatchTime(long ticks)
        {
            ticks /= 1000000;

            long h = ticks / 36000;
            ticks %= 36000;

            long m = ticks / 600;
            ticks %= 600;

            long s = ticks / 10;
            long ms = ticks %= 10;

            return $"{m:00}:{s:00}.{ms:0}";
        }

        public void Back()
        {
            if (swState == StopWatchState.Stop)
            {
            }
            else if (swState == StopWatchState.Start)
            {   //split
                stopwatchOffset += DateTime.Now.Ticks - stopWatchStart.Ticks;
                stopWatchStart = DateTime.Now;
                splits.Add(stopwatchOffset - lastSplit);
                lastSplit = stopwatchOffset;
            }
            else //we're paused so stop
            {
                //reset 
                splits = new List<long>();
                stopwatchOffset = 0;
                lastSplit = 0;
                swState = StopWatchState.Stop;
            }
        }

        public void Next()
        {
            if (swState == StopWatchState.Stop)
            {
                stopwatchOffset = 0;
                lastSplit = 0;
                stopWatchStart = DateTime.Now;
                swState = StopWatchState.Start;
            }
            else if (swState == StopWatchState.Start)
            {   //pause
                swState = StopWatchState.Pause;
                stopwatchOffset += DateTime.Now.Ticks - stopWatchStart.Ticks;
            }
            else //we're paused so resume
            {
                stopWatchStart = DateTime.Now;
                swState = StopWatchState.Start;
            }
        }
    }
}
