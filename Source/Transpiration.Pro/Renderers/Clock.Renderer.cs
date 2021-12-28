using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;

namespace Transpiration
{
    public partial class Clock : IMode
    {
        IFont fontClock = new Font6x8();
        IFont fontDate = new Font4x6();

        public int RenderDelay => 500;

        public void Update(MicroGraphics gl, ITemperatureSensor temperatureSensor)
        {
            if(showCalendar)
            {
                UpdateCalendar(gl);
                return;
            }

            gl.Clear();

            gl.CurrentFont = fontClock;
            // gl.DrawText(-1, 0, $"{DateTime.Now.ToString("hh")}");
            var hour = $"{DateTime.Now.ToString("hh").TrimStart('0')}";

            gl.DrawText(10, 0, hour, alignment: TextAlignment.Right);
            gl.DrawPixel(10, 2);
            gl.DrawPixel(10, 5);
            gl.DrawText(12, 0, $"{DateTime.Now.ToString("mm")}");

            gl.CurrentFont = fontDate;
            var ampm = (DateTime.Now.Hour > 11) ? "PM" : "AM";
            gl.DrawText(24, 2, ampm);

            gl.CurrentFont = fontDate;
            gl.DrawText(16, 10, $"{DateTime.Now.ToString("MMM").ToUpper()} {DateTime.Now.Day}", alignment: TextAlignment.Center);

            try
            {
                gl.DrawText(16, 18, $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")} C", alignment: TextAlignment.Center);
            }
            catch
            {
                
            }

            gl.Show();
        }

        void UpdateCalendar(MicroGraphics gl)
        {
            gl.Clear();

            gl.CurrentFont = fontDate;

            gl.DrawText(16, 1, DateTime.Now.ToString("ddd").ToUpper(), alignment: TextAlignment.Center);

            gl.DrawText(16, 9, $"{DateTime.Now.ToString("MMM").ToUpper()} {DateTime.Now.Day}", alignment: TextAlignment.Center);

            gl.DrawText(16, 17, $"{DateTime.Now.ToString("yyy")}", alignment: TextAlignment.Center);

            gl.Show();
        }

        
    }
}