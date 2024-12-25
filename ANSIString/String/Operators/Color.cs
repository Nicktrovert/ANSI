namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>
        /// Increment operator to increase the foreground color
        /// </summary>
        public static ANSIString operator ++(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            if (ANSIs.ColorMode == ANSIColorMode.Color8)
            {
                s.FGColor += 1;
                if (s.FGColor > 37) throw new Exception("The string can not be incremented any further.");
            }
            else
            {
                ANSIs.SetForegroundColor((short)(ANSIs.FG_R + 1), (short)(ANSIs.FG_G + 1), (short)(ANSIs.FG_B + 1));
            }
            return s;
        }
        /// <summary>
        /// Decrement operator to decrease the foreground color
        /// </summary>
        public static ANSIString operator --(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            if (ANSIs.ColorMode == ANSIColorMode.Color8)
            {
                s.FGColor -= 1;
                if (s.FGColor < 30) throw new Exception("The string can not be decremented any further.");
            }
            else
            {
                ANSIs.SetForegroundColor((short)(ANSIs.FG_R - 1), (short)(ANSIs.FG_G - 1), (short)(ANSIs.FG_B - 1));
            }
            return s;
        }


        /// <summary>
        /// Operator to set the foreground color using ConsoleColor
        /// </summary>
        public static ANSIString operator +(ANSIString ANSIs, ConsoleColor c)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.FGColor = (short)(30 + s.ConsoleColorToInt(c));
            var rgb = ANSIs.ConsoleColorToRGB(c);
            ANSIs.SetForegroundColor(rgb.Item1, rgb.Item2, rgb.Item3);
            return s;
        }
        /// <summary>
        /// Operator to set the background color using ConsoleColor
        /// </summary>
        public static ANSIString operator -(ANSIString ANSIs, ConsoleColor c)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.BGColor = (short)(40 + s.ConsoleColorToInt(c));
            var rgb = ANSIs.ConsoleColorToRGB(c);
            ANSIs.SetBackgroundColor(rgb.Item1, rgb.Item2, rgb.Item3);
            return s;
        }
    }
}
