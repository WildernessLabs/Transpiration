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
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Units;
using SimpleJpegDecoder;

namespace Transpiration
{
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        St7789 display;
        MicroGraphics gl;
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

          //  DrawSplashScreen();

            //Init clock
            transpiration = new Transpiration();
            transpiration.Init(gl, buzzer, tempSensor);
            transpiration.Show();

            //update intervals to test
            transpiration.WorkoutInterval = 240;
            transpiration.RestInterval = 30;

            _ = UpdateClock();
        }

        async Task UpdateClock()
        {
            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));

            Console.WriteLine($"WiFi status: {result}");

            var today = await ClockService.GetTime();

            Device.SetClock(today.Subtract(new TimeSpan(7, 0, 0)));
        }

        void Initialize()
        {
            Console.WriteLine("Set Clock...");

            //set a reasonable default date
            Device.SetClock(new DateTime(2021, 11, 1));

            Console.WriteLine("Initialize hardware...");

            var config = new SpiClockConfiguration(new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode0);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            Console.WriteLine("Initalize display driver");

            display = new St7789(device: Device, spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240, height: 240, ColorType.Format16bppRgb565);

            display.IgnoreOutOfBoundsPixels = true;

            Console.WriteLine("Initialize MicroGraphics");

            gl = new MicroGraphics(display)
            {
                Rotation = RotationType._90Degrees
            };

            Console.WriteLine("Initialize temp sensor");

            var i2cBus = Device.CreateI2cBus();
            var bme680 = new Bme680(i2cBus, (byte)Bme680.Addresses.Address_0x76);
            bme680.StartUpdating();

            tempSensor = bme680;
            

            Console.WriteLine("Initialize buttons");

            next = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullDown);
            back = new PushButton(Device, Device.Pins.D15, ResistorMode.InternalPullDown);
            select = new PushButton(Device, Device.Pins.D05, ResistorMode.InternalPullDown);
            menu = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);

            next.Clicked += Next_Clicked;
            back.Clicked += Back_Clicked;

            select.Clicked += Select_Clicked;
            menu.Clicked += Menu_Clicked;

            Console.WriteLine("Initialize piezo");

            buzzer = new PiezoSpeaker(Device, Device.Pins.D13);
        }

        private void Menu_Clicked(object sender, EventArgs e)
        {
            transpiration.ToggleMenu();
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

            display.Fill(Color.LawnGreen, false);
            gl.Show();

            sw.Stop();
            Console.WriteLine($"Fill: {sw.Elapsed}");

            gl.DrawRectangle(0, 0, 240, 240, Color.DarkViolet, true);
            gl.Show();

            sw.Stop();
            Console.WriteLine($"DrawRectangle: {sw.Elapsed}");

            sw.Restart();

            display.Clear(Color.YellowGreen);
            gl.Show();

            sw.Stop();
            Console.WriteLine($"Clear with color: {sw.Elapsed}");

            sw.Restart();

            for (int i = 0; i < 240; i++)
            {
                for (int j = 0; j < 240; j++)
                {
                    gl.DrawPixel(i, j, Color.LawnGreen);
                }
            }

            gl.Show();

            sw.Stop();

            Console.WriteLine($"GL Fill via DrawPixel: {sw.Elapsed}");

            sw.Restart();

            ushort col = Color.MediumTurquoise.Color16bppRgb565;

            for (int i = 0; i < 240; i++)
            {
                for (int j = 0; j < 240; j++)
                {
                    display.DrawPixel(i, j, col);
                }
            }

            gl.Show();

            sw.Stop();

            Console.WriteLine($"565 Fill via DrawPixel: {sw.Elapsed}");

            sw.Restart();

            var jpgData = LoadResource("meadow-logo.jpg");

            Console.WriteLine($"Loaded {jpgData.Length} bytes, decoding jpeg ...");

            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            Console.WriteLine($"Jpeg decoded is {jpg.Length} bytes");
            Console.WriteLine($"Width {decoder.Width}");
            Console.WriteLine($"Height {decoder.Height}");

            var jpgImage = new BufferRgb888(decoder.Width, decoder.Height, jpg);

            gl.Clear();

            int x = 0;
            int y = (240 - decoder.Height) / 2;

            //we can optimize this by not drawing uneeded pixels 
            gl.DrawRectangle(0, 0, 240, y, Color.White, true);
            gl.DrawRectangle(0, y + decoder.Height, 240, 240 - y - decoder.Height, Color.White, true);

            display.DrawBuffer(x, y, jpgImage);

            sw.Stop();

            Console.WriteLine($"Jpeg show: {sw.Elapsed}");

            display.Show();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Transpiration.ProjLab.{filename}";

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