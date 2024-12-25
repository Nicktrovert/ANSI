namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        // ANSI escape sequence introducer
        private const string SEQC = "\u001b[";

        // The actual text value of the ANSIString
        public string Value = "";
        // Indexer to access individual characters with ANSI formatting
        public string this[int i]
        {
            get => GetANSI() + Value[i] + SEQC + "0m";
            private set => throw new UnauthorizedAccessException();
        }

        // Constructor to initialize ANSIString with a specified color mode
        public ANSIString(ANSIColorMode colorMode = ANSIColorMode.Color256)
        {
            // Send an ANSI escape sequence to the console to enable 256-color mode
            Console.Write("\u001b[=19h");
            this.ColorMode = colorMode;
        }

        // Method to construct ANSI escape codes based on the properties of the ANSIString
        private string GetANSI()
        {
            string ANSIData = $"{SEQC}0m";
            if (ColorMode == ANSIColorMode.Color8) 
            {
                ANSIData += $"{SEQC}{FGColor}m";
                ANSIData += $"{SEQC}{BGColor}m";
            }
            else
            {
                ANSIData += $"{SEQC}38;2;{FG_R};{FG_G};{FG_B}m";
                ANSIData += $"{SEQC}48;2;{BG_R};{BG_G};{BG_B}m";
            }
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
