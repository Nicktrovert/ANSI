namespace ANSI.String
{
    public sealed partial class ANSIString
    {
        public static implicit operator string(ANSIString ANSIs) => ANSIs.GetANSI() + ANSIs.Value + SEQC + "0m";
        public static implicit operator ANSIString(string s)
        {
            ANSIString ANSIs = new ANSIString();
            ANSIs.Value = s;
            return ANSIs;
        }
    }
}
