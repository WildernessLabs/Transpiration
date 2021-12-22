using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace Transpiration
{
    public static class Settings
    {
        public static Color ThemeColor => Color.White; // FromHex("889291");

        public static int TimeZoneOffset => -8;

        public static MicroGraphics Canvas { get; set; }
    }
}
