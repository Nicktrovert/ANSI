namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        private ColorValue foreground = ColorValue.FromRgb(255, 255, 255);
        private ColorValue? background;
        private ANSIColorMode colorMode = ANSIColorMode.TrueColor;

        /// <summary>True when using the terminal's default background, including its configured opacity.</summary>
        public bool UsesDefaultBackground => background is null;

        /// <summary>Clears the explicit background color and restores the terminal's default (SGR 49).</summary>
        /// <remarks>Transparency is controlled by the terminal. Inverse styling can change the visible background.</remarks>
        public void ResetBackgroundColor() => background = null;

        /// <summary>The output color mode. Changing modes preserves the selected colors.</summary>
        public ANSIColorMode ColorMode
        {
            get => colorMode;
            set
            {
                if (!Enum.IsDefined(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown color mode.");
                colorMode = value;
            }
        }

        /// <summary>Sets an RGB foreground color. Each channel must be between 0 and 255.</summary>
        public void SetForegroundColor(int r, int g, int b) => foreground = ColorValue.FromRgb(r, g, b);

        /// <summary>Sets an RGB background color. Each channel must be between 0 and 255.</summary>
        public void SetBackgroundColor(int r, int g, int b) => background = ColorValue.FromRgb(r, g, b);

        /// <summary>Sets a foreground color from the standard 256-color palette (0–255).</summary>
        public void SetForegroundPaletteColor(int index) => foreground = ColorValue.FromPalette(index);

        /// <summary>Sets a background color from the standard 256-color palette (0–255).</summary>
        public void SetBackgroundPaletteColor(int index) => background = ColorValue.FromPalette(index);

        /// <summary>Sets a foreground color from a valid ConsoleColor value.</summary>
        public void SetForegroundColor(ConsoleColor color) => foreground = ColorValue.FromConsoleColor(color);

        /// <summary>Sets a background color from a valid ConsoleColor value.</summary>
        public void SetBackgroundColor(ConsoleColor color) => background = ColorValue.FromConsoleColor(color);

        // Immutable values keep RGB and palette representations synchronized.
        private readonly record struct ColorValue(byte R, byte G, byte B, byte PaletteIndex)
        {
            private static readonly (byte R, byte G, byte B)[] Palette = CreatePalette();

            public int BasicIndex => PaletteIndex < 16 ? PaletteIndex % 8 : FindNearest(R, G, B, 8);

            public static ColorValue FromRgb(int r, int g, int b)
            {
                ValidateByte(r, nameof(r));
                ValidateByte(g, nameof(g));
                ValidateByte(b, nameof(b));
                return new((byte)r, (byte)g, (byte)b, (byte)FindNearest(r, g, b, 256));
            }

            public static ColorValue FromPalette(int index)
            {
                ValidateByte(index, nameof(index));
                var rgb = Palette[index];
                return new(rgb.R, rgb.G, rgb.B, (byte)index);
            }

            public static ColorValue FromConsoleColor(ConsoleColor color)
            {
                if (!Enum.IsDefined(color))
                    throw new ArgumentOutOfRangeException(nameof(color), color, "Unknown console color.");
                // ConsoleColor stores blue in bit 0 and red in bit 2; ANSI reverses those bits.
                int value = (int)color;
                int index = (value & 8) | ((value & 1) << 2) | (value & 2) | ((value & 4) >> 2);
                return FromPalette(index);
            }

            private static void ValidateByte(int value, string name)
            {
                if (value is < 0 or > 255)
                    throw new ArgumentOutOfRangeException(name, value, "Value must be between 0 and 255.");
            }

            private static int FindNearest(int r, int g, int b, int count)
            {
                int nearest = 0;
                int bestDistance = int.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    var color = Palette[i];
                    int dr = r - color.R, dg = g - color.G, db = b - color.B;
                    int distance = dr * dr + dg * dg + db * db;
                    if (distance < bestDistance)
                    {
                        nearest = i;
                        bestDistance = distance;
                    }
                }
                return nearest;
            }

            private static (byte R, byte G, byte B)[] CreatePalette()
            {
                // Conventional xterm colors. Terminals can customize the first 16 entries.
                var colors = new (byte R, byte G, byte B)[256];
                (byte R, byte G, byte B)[] basic =
                [
                    (0, 0, 0), (128, 0, 0), (0, 128, 0), (128, 128, 0),
                    (0, 0, 128), (128, 0, 128), (0, 128, 128), (192, 192, 192),
                    (128, 128, 128), (255, 0, 0), (0, 255, 0), (255, 255, 0),
                    (0, 0, 255), (255, 0, 255), (0, 255, 255), (255, 255, 255)
                ];
                basic.CopyTo(colors, 0);
                byte[] levels = [0, 95, 135, 175, 215, 255];
                for (int r = 0; r < 6; r++)
                    for (int g = 0; g < 6; g++)
                        for (int b = 0; b < 6; b++)
                            colors[16 + 36 * r + 6 * g + b] = (levels[r], levels[g], levels[b]);
                for (int i = 232; i < 256; i++)
                {
                    byte level = (byte)(8 + 10 * (i - 232));
                    colors[i] = (level, level, level);
                }
                return colors;
            }
        }
    }
}
