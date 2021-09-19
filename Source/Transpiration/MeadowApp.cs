using System;
using System.IO;
using System.Reflection;
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
using SimpleJpegDecoder;
using static Meadow.Foundation.Displays.DisplayBase;

namespace Transpiration
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7789 display;
        GraphicsLibrary gl;
        PushButton next, back, select, menu;
        PiezoSpeaker buzzer;

        ITemperatureSensor tempSensor;

        Transpiration transpiration;

        public MeadowApp()
        {
            Initialize();

            //draw init text
            gl.Clear();
            gl.CurrentFont = new Font12x20();
            gl.DrawText(0, 0, "Initializing ...", color: Color.White);
            gl.Show();

            DrawSplashScreen();

            //Init clock
            transpiration = new Transpiration();
            transpiration.Init(gl, buzzer, tempSensor);
            transpiration.Show(gl);

            _ = UpdateClock();
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
            //set a reasonable default date
            MeadowApp.Device.SetClock(new DateTime(2021, 11, 1));

            Console.WriteLine("Initialize hardware...");

            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            Console.WriteLine("Initalize display driver");

            display = new St7789(device: Device, spiBus: spiBus,
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
            select.LongClickedThreshold = new TimeSpan(0, 0, 1);

            next.Clicked += Next_Clicked;
            back.Clicked += Back_Clicked;

            select.Clicked += Select_Clicked;
            select.LongClicked += Select_LongClicked;

            Console.WriteLine("Initialize piezo");

            buzzer = new PiezoSpeaker(Device, Device.Pins.D13);
        }

        private void Select_LongClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Long pressed");
            transpiration.ToggleMenu(gl);
        }

        private void Select_Clicked(object sender, EventArgs e)
        {
            transpiration.Select();
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            transpiration.Back();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            transpiration.Next();
        }

        void DrawSplashScreen()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var jpgData = LoadResource("meadow-logo.jpg");

            Console.WriteLine($"Loaded {jpgData.Length} bytes, decoding jpeg ...");

            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            Console.WriteLine($"Jpeg decoded is {jpg.Length} bytes");
            Console.WriteLine($"Width {decoder.Width}");
            Console.WriteLine($"Height {decoder.Height}");

            gl.Clear();
            gl.DrawRectangle(0, 0, 240, 240, Color.White, true);

            int x = 0;
            int y = (240 - decoder.Height) / 2;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                display.DrawPixel(x, y, r, g, b);

                x++;

                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }
            sw.Stop();

            Console.WriteLine($"Jpeg show: {sw.Elapsed}");

            display.Show();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Transpiration.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}