using System;
namespace Transpiration
{
    public partial class IntervalTimer
    {
        enum IntervalTimerState
        {
            Stop,
            Start
        }

        IntervalTimerState itState = IntervalTimerState.Stop;

        DateTime intervalStart = DateTime.MinValue;

        public int Interval1 { get; set; } = 180; //seconds
        public int Interval2 { get; set; } = 60; //seconds

        public void Back()
        {
            if (itState == IntervalTimerState.Start)
            {
                itState = IntervalTimerState.Stop;
            }
        }

        public void Next()
        {
            if (itState == IntervalTimerState.Stop)
            {
                itState = IntervalTimerState.Start;
                intervalStart = DateTime.Now;
            }
        }

        int GetInterval(long seconds)
        {
            return (int)(seconds / (Interval1 + Interval2) + 1);
        }

        string GetActiveTime(long seconds)
        {
            int cycleTotal = Interval1 + Interval2;

            var cycleTime = seconds % cycleTotal;

            int activeTime;

            if (cycleTime >= Interval1)
            {
                activeTime = 0;
            }
            else
            {
                activeTime = Interval1 - (int)cycleTime;
            }

            return $"{activeTime / 60}:{activeTime % 60:00}";
        }

        string GetRestTime(long seconds)
        {
            int cycleTotal = Interval1 + Interval2;

            var cycleTime = seconds % cycleTotal;

            int restTime;

            if (cycleTime < Interval1)
            {
                restTime = Interval2;
            }
            else
            {
                restTime = Interval2 - (int)cycleTime + Interval1;
            }

            return $"{restTime / 60}:{restTime % 60:00}";
        }

        string GetTotalTime(long seconds)
        {
            return $"{seconds / 60}:{seconds % 60:00}";
        }
    }
}
