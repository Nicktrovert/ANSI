namespace ANSI.String
{
    public sealed partial class ANSIString : ICloneable
    {
        public object Clone()
        {
            ANSIString ANSI = new ANSIString();
            ANSI.Value = this.Value;
            ANSI.Bold = this.Bold;
            ANSI.Italic = this.Italic;
            ANSI.Strikethrough = this.Strikethrough;
            ANSI.Underline = this.Underline;
            ANSI.Blink = this.Blink;
            ANSI.FGColor = this.FGColor;
            ANSI.BGColor = this.BGColor;
            ANSI.Dim = this.Dim;
            ANSI.Inverse = this.Inverse;
            return ANSI;
        }
    }
}
