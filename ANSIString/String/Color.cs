namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        // Foreground and background color codes
        private short FGColor = 37;
        private short BGColor = 40;

        // RGB values for foreground and background colors
        private short FG_R = 255;
        private short FG_G = 255;
        private short FG_B = 255;

        private short BG_R = 0;
        private short BG_G = 0;
        private short BG_B = 0;

        /// <summary>
        /// color mode (default is 256-color mode)
        /// </summary>
        public ANSIColorMode ColorMode = ANSIColorMode.Color256;

        /// <summary>
        /// Method to set the foreground color using RGB values
        /// </summary>
        /// <param name="r">Red (0-255)</param>
        /// <param name="g">Green (0-255)</param>
        /// <param name="b">Blue (0-255)</param>
        public void SetForegroundColor(short r, short g, short b)
        {
            if (r < 0) r = 0; if (g < 0) g = 0; if (b < 0) b = 0;
            if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;
            FG_R = r;
            FG_G = g;
            FG_B = b;
        }

        /// <summary>
        /// Method to set the background color using RGB values
        /// </summary>
        /// <param name="r">Red (0-255)</param>
        /// <param name="g">Green (0-255)</param>
        /// <param name="b">Blue (0-255)</param>
        public void SetBackgroundColor(short r, short g, short b)
        {
            if (r < 0) r = 0; if (g < 0) g = 0; if (b < 0) b = 0;
            if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;
            BG_R = r;
            BG_G = g;
            BG_B = b;
        }

        /// <summary>
        /// Convert <see cref="System.ConsoleColor"/> to integer
        /// </summary>
        /// <param name="c">The <see cref="System.ConsoleColor"/></param>
        /// <returns>Integer position of the color relative to the starting point in ANSI code</returns>
        private int ConsoleColorToInt(ConsoleColor c)
        {
            return c switch
            {
                ConsoleColor.Black => 0,
                ConsoleColor.Red => 1,
                ConsoleColor.Green => 2,
                ConsoleColor.Yellow => 3,
                ConsoleColor.Blue => 4,
                ConsoleColor.Magenta => 5,
                ConsoleColor.Cyan => 6,
                ConsoleColor.White => 7,
                _ => 7
            };
        }
        /// <summary>
        /// Convert <see cref="System.ConsoleColor"/> to RGB values
        /// </summary>
        /// <param name="c">The <see cref="System.ConsoleColor"/></param>
        /// <returns>Translation of the <see cref="System.ConsoleColor"/> to the matching RGB values</returns>
        private (short, short, short) ConsoleColorToRGB(ConsoleColor c)
        {
            return c switch
            {
                ConsoleColor.Black => (12, 12, 12),
                ConsoleColor.Red => (197, 15, 31),
                ConsoleColor.Green => (19, 161, 14),
                ConsoleColor.Yellow => (193, 156, 0),
                ConsoleColor.Blue => (0, 55, 218),
                ConsoleColor.Magenta => (136, 23, 152),
                ConsoleColor.Cyan => (58, 150, 221),
                ConsoleColor.White => (204, 204, 204),
                _ => (204, 204, 204)
            };
        }
    }
}
