namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        /// <summary>
        /// Implicit conversion from ANSIString to string
        /// </summary>
        public static implicit operator string(ANSIString ANSIs) => ANSIs.GetANSI() + ANSIs.Value + SEQC + "0m";
        /// <summary>
        /// Implicit conversion from string to ANSIString
        /// </summary>
        public static implicit operator ANSIString(string s)
        {
            ANSIString ANSIs = new ANSIString();
            ANSIs.Value = s;
            return ANSIs;
        }
    }
}
