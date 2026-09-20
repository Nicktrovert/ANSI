namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>
        /// Operator to reverse the string
        /// </summary>
        public static ANSIString operator ~(ANSIString ANSIs)
        {
            ArgumentNullException.ThrowIfNull(ANSIs);
            ANSIString s = ANSIs.Clone();
            var elements = new List<string>();
            var enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(s.Value);
            while (enumerator.MoveNext()) elements.Add(enumerator.GetTextElement());
            elements.Reverse();
            s.Value = string.Concat(elements);
            return s;
        }

        /// <summary>
        /// Operator to set the inverse colors
        /// </summary>
        public static ANSIString operator -(ANSIString ANSIs)
        {
            ArgumentNullException.ThrowIfNull(ANSIs);
            ANSIString s = ANSIs.Clone();
            s.Inverse = true;
            return s;
        }
    }
}
