namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        private const string SEQC = "\u001b[";

        public string Value = "";
        public string this[int i]
        {
            get => GetANSI() + Value[i] + SEQC + "0m";
            private set => throw new UnauthorizedAccessException();
        }

        public ANSIString(ANSIColorMode colorMode = ANSIColorMode.Color256)
        {
            Console.Write("\u001b[=19h");
            this.ColorMode = colorMode;
        }

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
