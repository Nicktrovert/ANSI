﻿namespace ANSI.String
{
    public sealed partial class ANSIString : ICloneable
    {
        /// <summary>
        /// Method to create a copy of the <see cref="ANSI.String.ANSIString"/> instance
        /// </summary>
        /// <returns>A clone of the <see cref="ANSI.String.ANSIString"/> instance as <see cref="object"/></returns>
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
