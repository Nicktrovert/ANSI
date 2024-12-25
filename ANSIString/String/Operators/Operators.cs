namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>
        /// Operator to reverse the string
        /// </summary>
        public static ANSIString operator ~(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.Value = new string(s.Value.Reverse().ToArray());
            return s;
        }

        /// <summary>
        /// Operator to set the inverse colors
        /// </summary>
        public static ANSIString operator -(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.Inverse = true;
            return s;
        }
    }
}
