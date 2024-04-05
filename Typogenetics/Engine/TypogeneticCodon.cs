namespace Typogenetics.Engine
{
    /// <summary>
    /// Represents a single cell in the table FIG. 87 on page 510.
    /// </summary>
    public class TypogeneticCodon
    {
        public bool IsPunctuation { get; }
        public NucleotideEnum FirstBase { get; }
        public NucleotideEnum SecondBase { get; }
        public DirectionEnum Direction { get; }
        public AminoAcidEnum AminoAcid { get; }

        public TypogeneticCodon(NucleotideEnum firstBase, NucleotideEnum secondBase, DirectionEnum direction, AminoAcidEnum aminoAcid)
        {
            FirstBase = firstBase;
            SecondBase = secondBase;
            Direction = direction;
            AminoAcid = aminoAcid;
            IsPunctuation = false;
        }

        public TypogeneticCodon(NucleotideEnum firstBase, NucleotideEnum secondBase)
        {
            FirstBase = firstBase;
            SecondBase = secondBase;
            IsPunctuation = true;
        }
    }
}
