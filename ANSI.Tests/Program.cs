using ANSI;
using ANSI.String;

const string Esc = "\u001b[";
int passed = 0;
(string Name, Action Run)[] tests =
[
    ("Terminal-default background in every mode", () =>
    {
        foreach (ANSIColorMode mode in Enum.GetValues<ANSIColorMode>())
        {
            var text = new ANSIString(mode) { Value = "Default" };
            string fg = mode switch
            {
                ANSIColorMode.Color8 => $"{Esc}37m",
                ANSIColorMode.Color256 => $"{Esc}38;5;15m",
                _ => $"{Esc}38;2;255;255;255m"
            };
            Equal(true, text.UsesDefaultBackground);
            Equal($"{Esc}0m{fg}{Esc}49mDefault{Esc}0m", text.ToString());
            Equal($"{Esc}0m{fg}{Esc}49mD{Esc}0m", text[0]);
            Equal(true, text.Clone().UsesDefaultBackground);
            Equal(true, (text + ConsoleColor.Red).UsesDefaultBackground);
            Equal(true, (~text).UsesDefaultBackground);
            var colored = text - ConsoleColor.Blue;
            Equal(false, colored.UsesDefaultBackground);
            Equal(true, text.UsesDefaultBackground);
            colored.ResetBackgroundColor();
            Equal(text.ToString(), colored.ToString());
            foreach (ANSIColorMode other in Enum.GetValues<ANSIColorMode>())
            {
                text.ColorMode = other;
                Equal(true, text.UsesDefaultBackground);
                Contains($"{Esc}49m", text.ToString());
            }
        }
        Equal(true, ((ANSIString)"Default").UsesDefaultBackground);
    }),
    ("Background setters and reset preserve valid state", () =>
    {
        foreach (ANSIColorMode mode in Enum.GetValues<ANSIColorMode>())
        {
            var text = new ANSIString(mode) { Value = "Background", Bold = true };
            text.SetForegroundColor(12, 34, 56);
            string defaultOutput = text.ToString();
            Action[] setters =
            [
                () => text.SetBackgroundColor(0, 0, 0),
                () => text.SetBackgroundPaletteColor(0),
                () => text.SetBackgroundColor(ConsoleColor.Black)
            ];
            foreach (var set in setters)
            {
                set();
                Equal(false, text.UsesDefaultBackground);
                Equal(false, text.ToString().Contains($"{Esc}49m"));
                var clone = text.Clone();
                Equal(false, clone.UsesDefaultBackground);
                text.ResetBackgroundColor();
                text.ResetBackgroundColor();
                Equal(true, text.UsesDefaultBackground);
                Equal(defaultOutput, text.ToString());
                Equal(false, clone.UsesDefaultBackground);
            }
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor(1, 2, 256));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundPaletteColor(-1));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor((ConsoleColor)99));
            Equal(true, text.UsesDefaultBackground);
            Equal(defaultOutput, text.ToString());
            text.SetBackgroundPaletteColor(67);
            string explicitOutput = text.ToString();
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor(256, 0, 0));
            Equal(false, text.UsesDefaultBackground);
            Equal(explicitOutput, text.ToString());
        }
    }),
    ("Default TrueColor and explicit conversions", () =>
    {
        var text = (ANSIString)"Hello";
        Equal(ANSIColorMode.TrueColor, text.ColorMode);
        text.SetForegroundColor(12, 34, 56);
        text.SetBackgroundColor(78, 90, 123);
        Equal($"{Esc}0m{Esc}38;2;12;34;56m{Esc}48;2;78;90;123mHello{Esc}0m", (string)text);
        Equal((string)text, text.ToString());
        Equal((string)text, $"{text}");
        Equal($"{Esc}0m{Esc}38;2;12;34;56m{Esc}48;2;78;90;123mH{Esc}0m", text[0]);
        Equal(2, typeof(ANSIString).GetMethods().Count(m => m.Name == "op_Explicit"));
        Equal(0, typeof(ANSIString).GetMethods().Count(m => m.Name == "op_Implicit"));
    }),
    ("All palette indices and mode round trips", () =>
    {
        for (int index = 0; index < 256; index++)
        {
            var text = new ANSIString(ANSIColorMode.Color256) { Value = "P" };
            text.SetForegroundPaletteColor(index);
            text.SetBackgroundPaletteColor(255 - index);
            string expected = $"{Esc}0m{Esc}38;5;{index}m{Esc}48;5;{255 - index}mP{Esc}0m";
            Equal(expected, text.ToString());
            text.ColorMode = ANSIColorMode.TrueColor;
            var fg = ExpectedPalette(index);
            var bg = ExpectedPalette(255 - index);
            Equal($"{Esc}0m{Esc}38;2;{fg.R};{fg.G};{fg.B}m{Esc}48;2;{bg.R};{bg.G};{bg.B}mP{Esc}0m", text.ToString());
            text.ColorMode = ANSIColorMode.Color8;
            text.ColorMode = ANSIColorMode.Color256;
            Equal(expected, text.ToString());
        }
    }),
    ("RGB quantization preserves source RGB", () =>
    {
        var text = new ANSIString();
        text.SetForegroundColor(96, 134, 176);
        text.SetBackgroundColor(118, 118, 118);
        string original = text.ToString();
        text.ColorMode = ANSIColorMode.Color256;
        Contains($"{Esc}38;5;67m", text.ToString());
        Contains($"{Esc}48;5;243m", text.ToString());
        text.ColorMode = ANSIColorMode.Color8;
        text.ColorMode = ANSIColorMode.TrueColor;
        Equal(original, text.ToString());
    }),
    ("All ConsoleColor mappings in all modes", () =>
    {
        int[] expectedIndices = [0, 4, 2, 6, 1, 5, 3, 7, 8, 12, 10, 14, 9, 13, 11, 15];
        foreach (ConsoleColor color in Enum.GetValues<ConsoleColor>())
        {
            int index = expectedIndices[(int)color];
            var text = new ANSIString(ANSIColorMode.Color256);
            text.SetForegroundColor(color);
            text.SetBackgroundColor(color);
            Contains($"{Esc}38;5;{index}m{Esc}48;5;{index}m", text.ToString());
            text.ColorMode = ANSIColorMode.Color8;
            Contains($"{Esc}{30 + index % 8}m{Esc}{40 + index % 8}m", text.ToString());
            text.ColorMode = ANSIColorMode.TrueColor;
            var rgb = ExpectedPalette(index);
            Contains($"{Esc}38;2;{rgb.R};{rgb.G};{rgb.B}m", text.ToString());
        }
    }),
    ("Cloning preserves all state and is independent", () =>
    {
        foreach (ANSIColorMode mode in Enum.GetValues<ANSIColorMode>())
        {
            var text = new ANSIString(mode)
            {
                Value = "Styled", Bold = true, Dim = true, Italic = true, Underline = true,
                Blink = true, Inverse = true, Invisible = true, Strikethrough = true
            };
            text.SetForegroundColor(23, 91, 203);
            text.SetBackgroundPaletteColor(237);
            string original = text.ToString();
            var clone = text.Clone();
            Equal(mode, clone.ColorMode);
            Equal(original, clone.ToString());
            Equal(original, ((ANSIString)((ICloneable)text).Clone()).ToString());
            Contains($"{Esc}1m{Esc}2m{Esc}3m{Esc}4m{Esc}5m{Esc}7m{Esc}8m{Esc}9m", original);
            clone.Value = "Other";
            clone.Invisible = false;
            clone.SetForegroundColor(1, 2, 3);
            clone.SetBackgroundPaletteColor(16);
            Equal(original, text.ToString());
        }
    }),
    ("Color operators update copies", () =>
    {
        foreach (ANSIColorMode mode in Enum.GetValues<ANSIColorMode>())
        {
            var text = new ANSIString(mode) { Value = "Original", Invisible = true };
            text.SetForegroundPaletteColor(2);
            text.SetBackgroundPaletteColor(3);
            string original = text.ToString();
            var expected = text.Clone();
            expected.SetForegroundColor(ConsoleColor.Red);
            Equal(expected.ToString(), (text + ConsoleColor.Red).ToString());
            expected = text.Clone();
            expected.SetBackgroundColor(ConsoleColor.Blue);
            Equal(expected.ToString(), (text - ConsoleColor.Blue).ToString());
            var alias = text;
            var previous = text++;
            Equal(original, previous.ToString());
            Equal(original, alias.ToString());
            expected = alias.Clone();
            if (mode == ANSIColorMode.TrueColor) expected.SetForegroundColor(1, 129, 1);
            else expected.SetForegroundPaletteColor(3);
            Equal(expected.ToString(), text.ToString());
            --text;
            Equal(original, text.ToString());
            var lower = alias.Clone();
            --lower;
            expected = alias.Clone();
            if (mode == ANSIColorMode.TrueColor) expected.SetForegroundColor(0, 127, 0);
            else expected.SetForegroundPaletteColor(1);
            Equal(expected.ToString(), lower.ToString());
            Equal(original, alias.ToString());
        }
    }),
    ("Increment boundaries", () =>
    {
        foreach (ANSIColorMode mode in new[] { ANSIColorMode.Color8, ANSIColorMode.Color256 })
        {
            var text = new ANSIString(mode);
            text.SetForegroundPaletteColor(mode == ANSIColorMode.Color8 ? 7 : 255);
            string before = text.ToString();
            Throws<InvalidOperationException>(() => text++);
            Equal(before, text.ToString());
            text.SetForegroundPaletteColor(0);
            before = text.ToString();
            Throws<InvalidOperationException>(() => text--);
            Equal(before, text.ToString());
        }
        var rgb = new ANSIString();
        rgb.SetForegroundColor(255, 0, 128);
        ++rgb;
        Contains($"{Esc}38;2;255;1;129m", rgb.ToString());
        rgb.SetForegroundColor(0, 255, 128);
        --rgb;
        Contains($"{Esc}38;2;0;254;127m", rgb.ToString());
    }),
    ("Invalid inputs leave state unchanged", () =>
    {
        var text = (ANSIString)"Valid";
        string original = text.ToString();
        Throws<ArgumentNullException>(() => text.Value = null!);
        Throws<ArgumentOutOfRangeException>(() => text.ColorMode = (ANSIColorMode)99);
        Throws<ArgumentOutOfRangeException>(() => _ = new ANSIString((ANSIColorMode)(-1)));
        foreach (int invalid in new[] { int.MinValue, -1, 256, int.MaxValue })
        {
            Throws<ArgumentOutOfRangeException>(() => text.SetForegroundColor(invalid, 0, 0));
            Throws<ArgumentOutOfRangeException>(() => text.SetForegroundColor(1, invalid, 0));
            Throws<ArgumentOutOfRangeException>(() => text.SetForegroundColor(1, 2, invalid));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor(invalid, 0, 0));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor(1, invalid, 0));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor(1, 2, invalid));
            Throws<ArgumentOutOfRangeException>(() => text.SetForegroundPaletteColor(invalid));
            Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundPaletteColor(invalid));
        }
        Throws<ArgumentOutOfRangeException>(() => text.SetForegroundColor((ConsoleColor)16));
        Throws<ArgumentOutOfRangeException>(() => text.SetBackgroundColor((ConsoleColor)(-1)));
        Throws<ArgumentOutOfRangeException>(() => _ = text + (ConsoleColor)16);
        Throws<ArgumentOutOfRangeException>(() => _ = text - (ConsoleColor)(-1));
        Equal(original, text.ToString());
        Throws<ArgumentNullException>(() => _ = (ANSIString)(string)null!);
        Throws<ArgumentNullException>(() => _ = (string)(ANSIString)null!);
        ANSIString missing = null!;
        Throws<ArgumentNullException>(() => missing++);
        Throws<ArgumentNullException>(() => missing--);
        Throws<ArgumentNullException>(() => _ = missing + ConsoleColor.Red);
        Throws<ArgumentNullException>(() => _ = missing - ConsoleColor.Red);
        Throws<ArgumentNullException>(() => _ = ~missing);
        Throws<ArgumentNullException>(() => _ = -missing);
    }),
    ("Unicode reversal and inversion preserve source", () =>
    {
        var text = (ANSIString)"A👩‍💻e\u0301";
        text.Invisible = true;
        string original = text.ToString();
        var reversed = ~text;
        Equal("e\u0301👩‍💻A", reversed.Value);
        Equal(true, reversed.Invisible);
        Equal(true, (-text).Inverse);
        Equal(original, text.ToString());
        Equal("", (~new ANSIString()).Value);
        Throws<IndexOutOfRangeException>(() => _ = text[-1]);
    }),
    ("Construction, conversions, and operators are silent", () =>
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;
        using var output = new StringWriter();
        using var error = new StringWriter();
        try
        {
            Console.SetOut(output);
            Console.SetError(error);
            foreach (ANSIColorMode mode in Enum.GetValues<ANSIColorMode>())
            {
                var text = new ANSIString(mode) { Value = "Silent" };
                text.SetForegroundPaletteColor(2);
                _ = text.Clone();
                _ = text + ConsoleColor.Red;
                _ = text - ConsoleColor.Blue;
                _ = ~text;
                _ = -text;
                ++text;
                --text;
                _ = text.ToString();
                _ = text[0];
                _ = (string)text;
                text.SetBackgroundPaletteColor(123);
                text.ResetBackgroundColor();
            }
            _ = (ANSIString)"Silent";
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
        Equal("", output.ToString());
        Equal("", error.ToString());
    })
];

foreach (var test in tests)
{
    try
    {
        test.Run();
        passed++;
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception error)
    {
        Console.Error.WriteLine($"FAIL {test.Name}: {error}");
    }
}
Console.WriteLine($"{passed}/{tests.Length} regression groups passed.");
return passed == tests.Length ? 0 : 1;

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"Expected '{expected}', got '{actual}'.");
}

static void Contains(string expected, string actual)
{
    if (!actual.Contains(expected, StringComparison.Ordinal))
        throw new Exception($"Expected '{actual}' to contain '{expected}'.");
}

static void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}.");
}

static (int R, int G, int B) ExpectedPalette(int index)
{
    if (index < 16)
    {
        (int R, int G, int B)[] basic =
        [
            (0, 0, 0), (128, 0, 0), (0, 128, 0), (128, 128, 0),
            (0, 0, 128), (128, 0, 128), (0, 128, 128), (192, 192, 192),
            (128, 128, 128), (255, 0, 0), (0, 255, 0), (255, 255, 0),
            (0, 0, 255), (255, 0, 255), (0, 255, 255), (255, 255, 255)
        ];
        return basic[index];
    }
    if (index >= 232)
    {
        int gray = 8 + (index - 232) * 10;
        return (gray, gray, gray);
    }
    int cube = index - 16;
    static int Channel(int step) => step == 0 ? 0 : 55 + 40 * step;
    return (Channel(cube / 36), Channel(cube / 6 % 6), Channel(cube % 6));
}
