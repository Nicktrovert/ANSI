namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>Explicitly renders an ANSIString as formatted text.</summary>
        public static explicit operator string(ANSIString text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return text.ToString();
        }

        /// <summary>Explicitly wraps non-null text using the default TrueColor mode.</summary>
        public static explicit operator ANSIString(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return new ANSIString { Value = text };
        }
    }
}
