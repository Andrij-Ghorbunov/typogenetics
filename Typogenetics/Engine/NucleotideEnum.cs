namespace Typogenetics.Engine
{
    public enum NucleotideEnum : byte
    {
        A = 0,
        C = 1,
        G = 2,
        T = 3
    }

    public static class Nucleotide
    {
        public static bool IsPyrimidine(NucleotideEnum n)
        {
            // C = 1, T = 3 - odd numbers
            return (byte)n % 2 == 1;
        }

        public static bool IsPurine(NucleotideEnum n)
        {
            // A = 0, G = 2 - even numbers
            return (byte)n % 2 == 0;
        }

        public static NucleotideEnum Complementary(NucleotideEnum n)
        {
            return (NucleotideEnum)(3 - (byte)n);
        }
    }
}
