using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TextDisplayMenu;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace Transpiration
{
    public class Transpiration
    {
        public enum TranspirationState
        {
            Clock,
            StopWatch,
            IntervalTimer,
        }

        const string ActiveID = "active";
        const string RestId = "rest";
        const string SetTime = "setTime";
        const string SetDate = "setDate";

        TranspirationState state = TranspirationState.Clock;

        PiezoSpeaker buzzer;

        Menu menu;

        Clock clock = new Clock();
        IntervalTimer intervalTimer = new IntervalTimer();
        Stopwatch stopwatch = new Stopwatch();

        ITemperatureSensor temperatureSensor;

        private FontBase fontClock;

        public int WorkoutInterval
        {
            get => intervalTimer.Interval1;
            set => intervalTimer.Interval1 = value;
        }

        public int RestInterval
        {
            get => intervalTimer.Interval2;
            set => intervalTimer.Interval2 = value;
        }

        public Transpiration()
        {

        }

        public void Init(GraphicsLibrary gl, PiezoSpeaker buzzer, ITemperatureSensor temperatureSensor)
        {
            this.buzzer = buzzer;
            this.temperatureSensor = temperatureSensor;
            fontClock = new Font12x20();
            InitMenu(gl);
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
                    subItems: new MenuItem[]{new MenuItem("Active", id: ActiveID, type: "TimeShort", value: new TimeSpan(0, 0, intervalTimer.Interval1)),
                                             new MenuItem("Rest", id: RestId, type: "TimeShort", value: new TimeSpan(0, 0, intervalTimer.Interval2))
                    })
             };

            menu = new Menu(gl, menuItems);

            menu.ValueChanged += Menu_ValueChanged;

            Console.WriteLine("Menu initialized");
        }

        public void SetMode(TranspirationState mode)
        {
            switch(mode)
            {
                case TranspirationState.Clock:
                    break;
                case TranspirationState.StopWatch:
                    break;
                case TranspirationState.IntervalTimer:
                    break;
                default:
                    return; //fail silently
            }

            state = mode;
        }

        private void Menu_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            switch (e.ItemID)
            {
                case ActiveID:
                    intervalTimer.Interval1 = ((int)((TimeSpan)e.Value).TotalSeconds);
                    break;
                case RestId:
                    intervalTimer.Interval2 = ((int)((TimeSpan)e.Value).TotalSeconds);
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

        public void Show(GraphicsLibrary gl)
        {
            Task.Run(() =>
            {
                while (menu.IsEnabled == false)
                {
                    Update(gl);
                }
            });
        }

        public void ToggleMenu(GraphicsLibrary gl)
        {
            if (menu.IsEnabled == false)
            {
                menu.Enable();
            }
            else if (menu.IsEnabled)
            {
                menu.Back();
                Show(gl);
            }
        }

        public void Update(GraphicsLibrary gl)
        {
            if (menu != null && menu.IsEnabled)
            {
                return;
            }

            gl.Clear();

            if (state == TranspirationState.Clock)
            {
                clock.Update(gl, temperatureSensor);
                Thread.Sleep(1000);
            }
            else if (state == TranspirationState.StopWatch)
            {
                stopwatch.Update(gl);
                Thread.Sleep(10);
            }
            else
            {
                intervalTimer.Update(gl);
                Thread.Sleep(10);
            }

            gl.Show();
        }

        public async Task AlarmBeep()
        {
            for (int i = 0; i < 4; i++)
            {
                await PlayTone(2000, 75).ConfigureAwait(false);
                await Task.Delay(25);
            }
        }

        public Task Beep()
        {
            return PlayTone(2000, 75);
        }

        public async Task PlayTone(int frequency, int duration)
        {
            buzzer.PlayTone(frequency);
            await Task.Delay(duration);
            buzzer.StopTone();
        }

        public void Back()
        {
            Beep();

            if (menu.IsEnabled)
            {
                menu.Previous();
                return;
            }

            if (state == TranspirationState.StopWatch)
            {
                stopwatch.Back();
            }
            else if (state == TranspirationState.IntervalTimer)
            {
                intervalTimer.Back();
            }
        }

        public void Next()
        {
            Beep();

            if (menu.IsEnabled)
            {
                menu.Next();
                return;
            }

            if (state == TranspirationState.StopWatch)
            {
                stopwatch.Next();
            }
            else if (state == TranspirationState.IntervalTimer)
            {
                intervalTimer.Next();
            }
        }

        public void Reset()
        {

        }

        public void Select()
        {
            Beep();

            if (menu.IsEnabled)
            {
                menu.Select();
                return;
            }

            switch (state)
            {
                case TranspirationState.Clock:
                    state = TranspirationState.StopWatch;
                    break;
                case TranspirationState.StopWatch:
                    state = TranspirationState.IntervalTimer;
                    break;
                default:
                    state = TranspirationState.Clock;
                    break;
            }
        }
    }
}
