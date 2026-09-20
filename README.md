# ANSIString

ANSIString is a C# library for creating and manipulating ANSI escape sequences for terminal text formatting. It allows you to easily add colors, styles, and other text attributes to your terminal output.

## Features

- Apply text colors (foreground and background)
- Add text styles (bold, italic, underline, etc.)
- Combine multiple styles and colors
- Reset text formatting

## Installation

- Download the latest version from [Releases](https://github.com/Nicktrovert/ANSI/releases/)
- Place the `ANSI.dll` file somewhere accessible
- add a reference to the `ANSI.dll` file in your solution.

## Usage

Here's a simple example of how to use ANSIString:

```csharp
using ANSI.String;

class Program
{
    static void Main()
    {
        ANSIString text = new ANSIString();
        text.Value = "Hello, World!";
        text.Bold = true;
        text.SetForegroundColor(255, 0, 0); // Red text
        Console.WriteLine(text);
    }
}
```

## Documentation
### Color modes

- `ANSIColorMode.TrueColor` is the default and emits 24-bit RGB sequences (`38;2;r;g;b` / `48;2;r;g;b`).
- `ANSIColorMode.Color256` emits indexed palette sequences (`38;5;n` / `48;5;n`). RGB setters choose the nearest color by squared RGB distance using the conventional xterm palette; ties use the lowest index.
- `ANSIColorMode.Color8` emits basic ANSI colors (`30–37` / `40–47`). Bright palette colors reduce to their basic counterparts; other palette colors map to the nearest basic color.

```csharp
using ANSI;
using ANSI.String;

ANSIString text = (ANSIString)"Hello, World!";
text.SetForegroundColor(255, 80, 20);
Console.WriteLine(text); // ToString() renders the formatted text.

text.ColorMode = ANSIColorMode.Color256;
text.SetForegroundPaletteColor(208); // Exact palette index, even for duplicate colors.
text.SetBackgroundPaletteColor(234);
string formatted = (string)text;
```

Changing modes preserves the selected color, including the original RGB values or explicit palette index. Palette colors use conventional xterm RGB values in TrueColor mode; terminals may customize the first 16 palette entries. Both foreground and background setters also accept all 16 `ConsoleColor` values.

### Terminal backgrounds and transparency

New strings use the terminal's default background (`ESC[49m`) in all color modes, rather than forcing black. This lets a terminal retain its configured transparency. The library cannot enable transparency on an opaque terminal; opacity depends on the terminal and its settings.

```csharp
var text = (ANSIString)"Terminal-default background";
Console.WriteLine(text.UsesDefaultBackground); // true
text.SetBackgroundColor(20, 30, 40);           // Select an explicit color.
text.ResetBackgroundColor();                 // Restore the terminal default.
Console.WriteLine(text);
```

RGB, palette, and ConsoleColor background setters all select an explicit background; `ResetBackgroundColor()` clears it. Cloning and changing color modes preserve this choice. Inverse styling swaps foreground and background and can therefore create a colored background even when `UsesDefaultBackground` is true. Text containing its own ANSI escape sequences can also override formatting.

TrueColor remains 24-bit RGB (8 bits per channel). There is no portable ANSI SGR text-color mode for 32-bit RGBA, per-cell alpha, or higher RGB precision. A common 32-bit RGBA value holds 24 bits of color plus 8 bits of opacity, not additional RGB precision. Terminal-specific opacity controls are separate from these color modes.

References: [XTerm SGR sequences and RGB ranges](https://invisible-island.net/xterm/ctlseqs/ctlseqs.html) and [kitty background opacity](https://sw.kovidgoyal.net/kitty/conf/#opt-kitty.background_opacity).

### Validation and conversions

`Value`, `ColorMode`, and the decoration flags are properties. Null text throws `ArgumentNullException`; unknown enums and RGB channels or palette indices outside 0–255 throw `ArgumentOutOfRangeException`. Failed setters leave the existing state intact.

String conversions now require explicit casts: `(ANSIString)"text"` and `(string)text`. `ToString()`, interpolation, and `Console.WriteLine(text)` render ANSI formatting. Constructing, cloning, converting, and formatting objects never write to the console themselves.

`Clone()` copies every property and color. Operators return independent copies: `+ ConsoleColor` sets foreground, `- ConsoleColor` sets background, unary `-` enables inverse, and `~` reverses Unicode text elements. `++` / `--` advance or decrease the foreground palette index in Color8/Color256 and throw `InvalidOperationException` at the palette boundaries. In TrueColor they change each RGB channel by one, saturating at 0 and 255. The indexer continues to address individual UTF-16 characters.

**Migration:** rebuild consumers because public fields are now properties and some method signatures have changed. Replace implicit string assignments with casts. Use `TrueColor` for the previous RGB behavior of `Color256`; invalid RGB values now throw instead of being silently clamped. Backgrounds now default to the terminal's background; call `SetBackgroundColor(0, 0, 0)` to retain the previous explicit black background.

For detailed documentation and examples, please refer to the source code and comments within the code.

## Verification

Build and run the dependency-free regression executable (it exits nonzero on failure):

```sh
dotnet build ANSI.sln
dotnet run --project ANSI.Tests/ANSI.Tests.csproj
```

The executable checks emitted escape sequences, all 256 palette entries, mode changes, all ConsoleColor mappings, cloning, operator boundaries, invalid inputs, explicit conversions, Unicode reversal, and absence of console side effects. It is a console test harness, not a `dotnet test` project.

## Contributing
There are currently no set guidelines for contributions.

## License
ANSIString is licensed under the MIT License. See the [License](https://github.com/Nicktrovert/ANSI/blob/master/LICENSE) file for more information.
