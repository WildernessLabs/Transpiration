using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using System;
using System.Globalization;

namespace Transpiration
{
    public partial class Clock
    {
        public void Update(GraphicsLibrary gl, ITemperatureSensor temperatureSensor)
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
    }
}