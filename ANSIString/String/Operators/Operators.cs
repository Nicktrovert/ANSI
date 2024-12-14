namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        public static ANSIString operator ~(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.Value = new string(s.Value.Reverse().ToArray());
            return s;
        }

        public static ANSIString operator -(ANSIString ANSIs)
        {
            ANSIString s = (ANSIString)ANSIs.Clone();
            s.Inverse = true;
            return s;
        }
    }
}
