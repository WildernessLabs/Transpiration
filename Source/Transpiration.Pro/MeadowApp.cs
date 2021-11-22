using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Transpiration.Services;

namespace Transpiration
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        Max7219 display;
        GraphicsLibrary gl;
        PushButton next, back, select, menu;
        PiezoSpeaker buzzer;

        Tmp102 tempSensor;

        BLEService bleService;

        Transpiration transpiration;

        public MeadowApp()
        {
            Initialize();

            //draw init text
            gl.Clear();
            gl.DrawLine(0, 0, 4, 4, true);
            gl.CurrentFont = new Font4x6();
            gl.DrawText(0, 0, "Init");
            gl.Show();

            //Init transpiration 
            transpiration = new Transpiration();
            transpiration.Init(gl, buzzer, tempSensor);
            transpiration.Show(gl);

            //update intervals to test
            transpiration.WorkoutInterval = 240;
            transpiration.RestInterval = 30;

            //  _ = UpdateClock();

            bleService = new BLEService();
            bleService.Initialize(Device);

            //event coordination between the BLE service and the transpiration logic
            bleService.RestIntervalChanged += (c, o) =>
            {
                Console.WriteLine($"Rest interval changed {(Int32)o}");
                transpiration.RestInterval = (Int32)o;
            };

            bleService.WorkoutIntervalChanged += (c, o) =>
            {
                Console.WriteLine($"Rest interval changed {(Int32)o}");
                transpiration.WorkoutInterval = (Int32)o;
            };

            bleService.ModeChanged += (c, o) =>
            {
                Console.WriteLine($"Mode changed {(Int32)o}");
                transpiration.SetMode((Transpiration.TranspirationState)o);
            };
        }

        void Initialize()
        {
            //set a reasonable default date
            MeadowApp.Device.SetClock(new DateTime(2021, 12, 1));

            Console.WriteLine("Initalize display driver");

            display = new Max7219(Device,
                 Device.CreateSpiBus(),
                 Device.Pins.D00,
                 4, 3,
                 Max7219.Max7219Type.Display);

            display.IgnoreOutOfBoundsPixels = true;

            display.SetBrightness(0);
            display.SetBrightness(6, 0);
            display.SetBrightness(6, 1);
            display.SetBrightness(6, 2);
            display.SetBrightness(6, 3);

            Console.WriteLine("Initialize MicroGraphics");

            gl = new GraphicsLibrary(display);
            gl.Rotation = RotationType._90Degrees;

            gl.Clear();

            gl.Show();

            display.IgnoreOutOfBoundsPixels = true;

            Console.WriteLine("Initialize temp sensor");

            //Init temp sensor
            tempSensor = new Tmp102(Device.CreateI2cBus());

            var result = tempSensor.Read();
            result.Wait();

            Console.WriteLine("Current temp" + result.Result.Celsius);

            Console.WriteLine("Initialize buttons");

            next = new PushButton(Device, Device.Pins.D11, ResistorMode.InternalPullDown);
            back = new PushButton(Device, Device.Pins.D12, ResistorMode.InternalPullDown);
            select = new PushButton(Device, Device.Pins.D13, ResistorMode.InternalPullDown);
            menu = new PushButton(Device, Device.Pins.D14, ResistorMode.InternalPullDown);
            
            next.Clicked += Next_Clicked;
            back.Clicked += Back_Clicked;
            select.Clicked += Select_Clicked;
            menu.Clicked += MenuClicked;

            Console.WriteLine("Initialize piezo");

            buzzer = new PiezoSpeaker(Device, Device.Pins.D10);
        }

        private void MenuClicked(object sender, EventArgs e)
        {
            Console.WriteLine("MenuClicked");
            transpiration.ToggleMenu(gl);
        }

        private void Select_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("SelectClicked");
            transpiration.Select();
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("BackClicked");
            transpiration.Back();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("NextClicked");
            transpiration.Next();
        }
    }
}