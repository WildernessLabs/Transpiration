using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using static Meadow.Foundation.Displays.DisplayBase;

namespace Transpiration
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        GraphicsLibrary gl;
        PushButton next, back, select, menu;
        PiezoSpeaker buzzer;

        ITemperatureSensor tempSensor;

        Clock clock;

        public MeadowApp()
        {
            Initialize();

            //draw init text
            gl.Clear();
            gl.CurrentFont = new Font12x20();
            gl.DrawText(0, 0, "Initializing ...", color: Color.White);
            gl.Show();

            //Init clock
            clock = new Clock();
            clock.Init(gl, tempSensor);

            _ = UpdateClock();

            Task.Run(() =>
            {
                while(true)
                {
                    clock.Update(gl);
                }
            });
        }

        async Task UpdateClock()
        {
            await Device.InitWiFiAdapter();

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

            var today = await ClockService.GetTime();

            Device.SetClock(today.Subtract(new TimeSpan(7, 0, 0)));
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            Console.WriteLine("Initalize display driver");

            var display = new St7789(device: Device, spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240, displayColorMode: DisplayColorMode.Format12bppRgb444);

            display.IgnoreOutOfBoundsPixels = true;

            Console.WriteLine("Initialize MicroGraphics");

            gl = new GraphicsLibrary(display);

            Console.WriteLine("Initialize temp sensor");

            //Init temp sensor
            tempSensor = new AnalogTemperature(Device, Device.Pins.A01, AnalogTemperature.KnownSensorType.LM35);
            (tempSensor as AnalogTemperature).StartUpdating(new TimeSpan(0, 5, 0));

            Console.WriteLine("Initialize buttons");

            next = new PushButton(Device, Device.Pins.D03, ResistorMode.InternalPullUp);
            back = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullUp);
            select = new PushButton(Device, Device.Pins.D04, ResistorMode.InternalPullUp);

            next.Clicked += Next_Clicked;
            back.Clicked += Back_Clicked;

            select.Clicked += Select_Clicked;
            select.LongClicked += Select_LongClicked;

            Console.WriteLine("Initialize piezo");

            buzzer = new PiezoSpeaker(Device, Device.Pins.D02);
        }

        private void Select_LongClicked(object sender, EventArgs e)
        {
            clock.Left();
        }

        private void Select_Clicked(object sender, EventArgs e)
        {
            clock.Right();
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Back pressed");
            clock.Up();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Next pressed");
            clock.Down();
        }
    }
}