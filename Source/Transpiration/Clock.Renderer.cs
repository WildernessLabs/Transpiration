using Meadow.Foundation;
using Meadow.Foundation.Displays.TextDisplayMenu;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Transpiration
{
    public partial class Clock
    {
        FontBase fontClock;

        Menu menu;

        const string ActiveID = "active";
        const string RestId = "rest";
        const string SetTime = "setTime";
        const string SetDate = "setDate";

        ITemperatureSensor temperatureSensor;

        public void Init(GraphicsLibrary gl, ITemperatureSensor tempSensor)
        {
            temperatureSensor = tempSensor;

            fontClock = new Font12x20();

            MeadowApp.Device.SetClock(new DateTime(2021, 3, 1));

            InitMenu(gl);

            return; 
        }

        public void Update(GraphicsLibrary gl)
        {
            if (menu != null && menu.IsEnabled)
            {
                return;
            }

            gl.Clear();

            if(state == ClockState.Clock)
            {
                UpdateClock(gl);
                Thread.Sleep(1000);
            }
            else if(state == ClockState.StopWatch)
            {
                UpdateStopWatch(gl);
                Thread.Sleep(0);
            }
            else
            {
                UpdateIntervalTimer(gl);
                Thread.Sleep(0);
            }

            gl.Show();
        }

        void UpdateClock(GraphicsLibrary gl)
        {
            var time = DateTime.Now.ToString("h:mm", CultureInfo.InvariantCulture);
            gl.DrawText(0, 40, time,
                GraphicsLibrary.ScaleFactor.X3, GraphicsLibrary.TextAlignment.Left);

            gl.DrawText(36*time.Length, 56, DateTime.Now.ToString("tt", CultureInfo.InvariantCulture),
                GraphicsLibrary.ScaleFactor.X2, GraphicsLibrary.TextAlignment.Left);

            gl.DrawText(10, 110, DateTime.Now.ToString("ddd, MMM d", CultureInfo.InvariantCulture),
                GraphicsLibrary.ScaleFactor.X1, GraphicsLibrary.TextAlignment.Left);

            var tempString = $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")}°C";

            gl.DrawText(10, 140, tempString,
                GraphicsLibrary.ScaleFactor.X1, GraphicsLibrary.TextAlignment.Left);
        }

        void UpdateStopWatch(GraphicsLibrary gl)
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

        void UpdateIntervalTimer(GraphicsLibrary gl)
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

        

        int GetInterval(long seconds)
        {
            return (int)(seconds / (interval1 + interval2) + 1);
        }

        string GetTotalTime(long seconds)
        {
            return $"{seconds / 60}:{seconds % 60:00}";
        }

        string GetActiveTime(long seconds)
        {
            int cycleTotal = interval1 + interval2;

            var cycleTime = seconds % cycleTotal;

            int activeTime;

            if(cycleTime >=interval1)
            {
                activeTime = 0;
            }
            else
            {
                activeTime = interval1 - (int)cycleTime;
            }

            return $"{activeTime / 60}:{activeTime % 60:00}";
        }

        string GetRestTime(long seconds)
        {
            int cycleTotal = interval1 + interval2;

            var cycleTime = seconds % cycleTotal;

            int restTime;

            if (cycleTime < interval1)
            {
                restTime = interval2;
            }
            else
            {
                restTime = interval2 - (int)cycleTime + interval1;
            }

            return $"{restTime / 60}:{restTime % 60:00}";
        }  

        void InitMenu(GraphicsLibrary gl)
        {
            Console.WriteLine("InitMenu");

            var menuItems = new MenuItem[]
            {
                new MenuItem("Clock",
                    subItems: new MenuItem[]{new MenuItem("Date", id: SetDate, type: "Date", value: new DateTime(2021, 2, 20)),
                                             new MenuItem("Time", id: SetTime, type: "Time", value: new TimeSpan(12, 45, 30))
                    }),
                new MenuItem("Interval",
                    subItems: new MenuItem[]{new MenuItem("Active", id: ActiveID, type: "TimeShort", value: new TimeSpan(0, 0, interval1)),
                                             new MenuItem("Rest", id: RestId, type: "TimeShort", value: new TimeSpan(0, 0, interval2)) 
                    })
             };

            menu = new Menu(gl, menuItems);

            menu.ValueChanged += Menu_ValueChanged;

            Console.WriteLine("Menu initialized");
        }

        private void Menu_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            switch (e.ItemID)
            {
                case ActiveID:
                    interval1 = (int)((TimeSpan)e.Value).TotalSeconds;
                    break;
                case RestId:
                    interval2 = (int)((TimeSpan)e.Value).TotalSeconds;
                    break;
                case SetTime:
                    { 
                        var now = DateTime.Now;
                        var time = (TimeSpan)e.Value;
                        MeadowApp.Device.SetClock(new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds));
                    }
                    break;
                case SetDate:
                    {
                        var now = DateTime.Now;
                        var date = (DateTime)e.Value;
                        MeadowApp.Device.SetClock(new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second));
                    }
                    break;
            }
        }
    }
}