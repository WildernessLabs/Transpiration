using System;

namespace Transpiration;

public partial class Timer
{
    StopWatchState timerState = StopWatchState.Stop;

    DateTime timerStart = DateTime.MinValue;

    long timerOffset; //if we pause

    string GeTimerTime(long ticks)
    {
        ticks /= 1000000;

        long h = ticks / 36000;
        ticks %= 36000;

        long m = ticks / 600;
        ticks %= 600;

        long s = ticks / 10;

        return $"{h:00}:{m:00}:{s:00}";
    }

    public void Back()
    {
        if (timerState == StopWatchState.Stop)
        {
        }

        else if (timerState == StopWatchState.Start)
        {
            timerOffset += DateTime.Now.Ticks - timerStart.Ticks;
            timerStart = DateTime.Now;
        }
        else //we're paused so stop
        {
            //reset 
            timerOffset = 0;
            timerState = StopWatchState.Stop;
        }
    }

    public void Next()
    {
        if (timerState == StopWatchState.Stop)
        {
            timerOffset = 0;
            timerStart = DateTime.Now;
            timerState = StopWatchState.Start;
        }
        else if (timerState == StopWatchState.Start)
        {   //pause
            timerState = StopWatchState.Pause;
            timerOffset += DateTime.Now.Ticks - timerStart.Ticks;
        }
        else //we're paused so resume
        {
            timerStart = DateTime.Now;
            timerState = StopWatchState.Start;
        }
    }
}