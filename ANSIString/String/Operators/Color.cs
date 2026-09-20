namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>Advances the foreground palette index, or increases RGB channels with saturation.</summary>
        public static ANSIString operator ++(ANSIString text) => ShiftForeground(text, 1);

        /// <summary>Decreases the foreground palette index, or decreases RGB channels with saturation.</summary>
        public static ANSIString operator --(ANSIString text) => ShiftForeground(text, -1);

        private static ANSIString ShiftForeground(ANSIString text, int delta)
        {
            ArgumentNullException.ThrowIfNull(text);
            ANSIString result = text.Clone();
            if (text.ColorMode == ANSIColorMode.TrueColor)
            {
                result.SetForegroundColor(
                    Math.Clamp(text.foreground.R + delta, 0, 255),
                    Math.Clamp(text.foreground.G + delta, 0, 255),
                    Math.Clamp(text.foreground.B + delta, 0, 255));
            }
            else
            {
                int index = text.ColorMode == ANSIColorMode.Color8
                    ? text.foreground.BasicIndex : text.foreground.PaletteIndex;
                int maximum = text.ColorMode == ANSIColorMode.Color8 ? 7 : 255;
                if (index + delta < 0 || index + delta > maximum)
                    throw new InvalidOperationException("The foreground color has reached the palette boundary.");
                result.SetForegroundPaletteColor(index + delta);
            }
            return result;
        }

        /// <summary>Returns a copy with the specified foreground color.</summary>
        public static ANSIString operator +(ANSIString text, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(text);
            ANSIString result = text.Clone();
            result.SetForegroundColor(color);
            return result;
        }

        /// <summary>Returns a copy with the specified background color.</summary>
        public static ANSIString operator -(ANSIString text, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(text);
            ANSIString result = text.Clone();
            result.SetBackgroundColor(color);
            return result;
        }
    }
}
