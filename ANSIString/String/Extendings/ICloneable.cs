namespace ANSI.String
{
    public sealed partial class ANSIString : ICloneable
    {
        /// <summary>Copies all text, color, mode, and decoration state.</summary>
        public ANSIString Clone() => (ANSIString)MemberwiseClone();

        object ICloneable.Clone() => Clone();
    }
}
