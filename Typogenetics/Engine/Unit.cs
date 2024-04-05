namespace Typogenetics.Engine
{
    /// <summary>
    /// A single position in a strand that can be occupied by one of the nucleotide bases.
    /// </summary>
    public class Unit
    {
        public NucleotideEnum Base { get; set; }

        /// <summary>
        /// The unit to the left of this one (null if none).
        /// </summary>
        public Unit? Left { get; set; }

        /// <summary>
        /// The unit to the right of this one (null if none).
        /// </summary>
        public Unit? Right { get; set; }

        /// <summary>
        /// The unit complementary to this one (null if none).
        /// </summary>
        public Unit? Complementary { get; set; }
    }
}
