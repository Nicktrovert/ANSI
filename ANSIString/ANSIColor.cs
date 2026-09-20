namespace ANSI
{
    /// <summary>
    /// Enum representing the color modes supported by ANSIString
    /// </summary>
    public enum ANSIColorMode
    {
        Color8 = 0,     // Basic ANSI colors (30–37 / 40–47)
        Color256 = 1,   // Indexed palette (38;5 / 48;5)
        TrueColor = 2,  // 24-bit RGB (38;2 / 48;2); default for ANSIString
    }
}
