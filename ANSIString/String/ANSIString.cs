namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        // ANSI escape sequence introducer
        private const string SEQC = "\u001b[";

        // The actual text value of the ANSIString
        private string value = "";
        public string Value
        {
            get => value;
            set => this.value = value ?? throw new ArgumentNullException(nameof(value));
        }
        // Indexer to access individual characters with ANSI formatting
        public string this[int i]
        {
            get => GetANSI() + Value[i] + SEQC + "0m";
        }

        // Constructor to initialize ANSIString with a specified color mode
        public ANSIString(ANSIColorMode colorMode = ANSIColorMode.TrueColor)
        {
            this.ColorMode = colorMode;
        }

        /// <summary>Returns the formatted text, with a reset before and after it.</summary>
        public override string ToString() => GetANSI() + Value + SEQC + "0m";

        // Method to construct ANSI escape codes based on the properties of the ANSIString
        private string GetANSI()
        {
            string ANSIData = $"{SEQC}0m";
            if (ColorMode == ANSIColorMode.Color8) 
            {
                ANSIData += $"{SEQC}{30 + foreground.BasicIndex}m";
            }
            else if (ColorMode == ANSIColorMode.Color256)
            {
                ANSIData += $"{SEQC}38;5;{foreground.PaletteIndex}m";
            }
            else
            {
                ANSIData += $"{SEQC}38;2;{foreground.R};{foreground.G};{foreground.B}m";
            }
            ANSIData += background is not { } bg ? $"{SEQC}49m" : ColorMode switch
            {
                ANSIColorMode.Color8 => $"{SEQC}{40 + bg.BasicIndex}m",
                ANSIColorMode.Color256 => $"{SEQC}48;5;{bg.PaletteIndex}m",
                _ => $"{SEQC}48;2;{bg.R};{bg.G};{bg.B}m"
            };
            if (Bold) ANSIData += $"{SEQC}1m";
            if (Dim) ANSIData += $"{SEQC}2m";
            if (Italic) ANSIData += $"{SEQC}3m";
            if (Underline) ANSIData += $"{SEQC}4m";
            if (Blink) ANSIData += $"{SEQC}5m";
            if (Inverse) ANSIData += $"{SEQC}7m";
            if (Invisible) ANSIData += $"{SEQC}8m";
            if (Strikethrough) ANSIData += $"{SEQC}9m";

            return ANSIData;
        }
    }
}
