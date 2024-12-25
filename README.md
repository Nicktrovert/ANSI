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
For detailed documentation and examples, please refer to the source code and comments within the code.

## Contributing
There are currently no set guidelines for contributions.

## License
ANSIString is licensed under the MIT License. See the [License](https://github.com/Nicktrovert/ANSI/blob/master/LICENSE) file for more information.