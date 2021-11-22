using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;

namespace Transpiration
{
    public partial class Clock
    {
        FontBase fontClock;
        FontBase fontDate;

        public void Update(GraphicsLibrary gl, ITemperatureSensor temperatureSensor)
        {
            if(fontClock == null)
            {
                fontDate = new Font4x6();
                fontClock = new Font6x8();
            }

            gl.Clear();

            gl.CurrentFont = fontClock;
            // gl.DrawText(-1, 0, $"{DateTime.Now.ToString("hh")}");
            var hour = $"{DateTime.Now.ToString("hh").TrimStart('0')}";

            gl.DrawText(11, 0, hour, alignment: GraphicsLibrary.TextAlignment.Right);
            gl.DrawPixel(11, 2);
            gl.DrawPixel(11, 5);
            gl.DrawText(13, 0, $"{DateTime.Now.ToString("mm")}");

            gl.CurrentFont = fontDate;
            var ampm = (DateTime.Now.Hour > 11) ? "PM" : "AM";
            gl.DrawText(25, 2, ampm);

            gl.CurrentFont = fontDate;
            gl.DrawText(16, 10, $"{DateTime.Now.ToString("MMM").ToUpper()} {DateTime.Now.Day}", alignment: GraphicsLibrary.TextAlignment.Center);

            try
            {
                gl.DrawText(16, 18, $"{temperatureSensor?.Temperature.Value.Celsius.ToString("0.0")} C", alignment: GraphicsLibrary.TextAlignment.Center);
            }
            catch
            {

            }

            gl.Show();

        }
    }
}