using System;
using System.Collections.Generic;
using System.Linq;

namespace Typogenetics.Engine
{
    /// <summary>
    /// Static class for converting strands into enzymes.
    /// </summary>
    public static class Ribosome
    {
        private const string Code = @"
AA pun
AC cut s
AG del s
AT swi r
CA mvr s
CC mvl s
CG cop r
CT off l
GA ina s
GC inc r
GG ing r
GT int l
TA rpy r
TC rpu l
TG lpy l
TT lpu l";

        private static readonly TypogeneticCodon[] Codons = new TypogeneticCodon[16];

        /// <summary>
        /// Pack 2 nucleotides into 1 number for a better array indexation.
        /// </summary>
        /// <param name="firstBase">First nucleotide base.</param>
        /// <param name="secondBase">Second nucleotide base.</param>
        /// <returns>A single number between 0 and 15.</returns>
        private static int BasesToIndex(NucleotideEnum firstBase, NucleotideEnum secondBase)
        {
            return 4 * (int)firstBase + (int)secondBase;
        }

        private static bool _isInitialized = false;

        private static void Init()
        {
            if (_isInitialized) return;
            var lines = Code.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                var firstBase = Enum.Parse<NucleotideEnum>(parts[0][0..1]);
                var secondBase = Enum.Parse<NucleotideEnum>(parts[0][1..2]);
                var instruction = parts[1] == "pun"
                    ? new TypogeneticCodon(firstBase, secondBase)
                    : new TypogeneticCodon(firstBase, secondBase, ParseDirection(parts[2]), Enum.Parse<AminoAcidEnum>(parts[1], true));
                Codons[BasesToIndex(firstBase, secondBase)] = instruction;
            }
            _isInitialized = true;
        }

        private static DirectionEnum ParseDirection(string s)
        {
            switch (s[0])
            {
                case 's': return DirectionEnum.Straight;
                case 'l': return DirectionEnum.Left;
                case 'r': return DirectionEnum.Right;
            }
            throw new Exception("Unknown direction");
        }

        public static List<Enzyme> Construct(Strand strand)
        {
            Init();
            var r = new List<Enzyme>();
            var aminoAcids = new List<AminoAcidEnum>();
            var direction = 0;
            if (strand.Count < 2) return r;
            var firstBase = strand[0];
            var secondBase = strand[1];
            var index = 0;
            var lastDirection = 0;
            var isFirstAmino = true;
            while (true)
            {
                var instruction = Codons[BasesToIndex(firstBase, secondBase)];
                if (instruction.IsPunctuation)
                {
                    if (aminoAcids.Any())
                    {
                        if (aminoAcids.Count > 1) direction -= lastDirection;
                        r.Add(new Enzyme(DirectionToNucleotide(direction), aminoAcids.ToArray()));
                    }
                    direction = 0;
                    lastDirection = 0;
                    isFirstAmino = true;
                    aminoAcids.Clear();
                }
                else
                {
                    // we're simply adding directions as numbers, every +1 is 90 degrees to the left
                    // and -1 is 90 degrees to the right
                    // Also, we need to add all but the first and the last,
                    // so we skip first and later subtract last (if it is different than the first)
                    lastDirection = (int)instruction.Direction;
                    if (!isFirstAmino)
                        direction += lastDirection;
                    aminoAcids.Add(instruction.AminoAcid);
                    isFirstAmino = false;
                }
                index += 2;
                if (index + 1 >= strand.Count) // end of strand
                {
                    if (aminoAcids.Any())
                    {
                        if (aminoAcids.Count > 1) direction -= lastDirection;
                        r.Add(new Enzyme(DirectionToNucleotide(direction), aminoAcids.ToArray()));
                    }
                    return r;
                }
                firstBase = strand[index];
                secondBase = strand[index + 1];
            }
        }

        private static readonly NucleotideEnum[] DirectionToNucleotideArray = new[]
        {
            NucleotideEnum.A,
            NucleotideEnum.C,
            NucleotideEnum.T,
            NucleotideEnum.G
        };

        private static NucleotideEnum DirectionToNucleotide(int totalDirection)
        {
            var r = totalDirection % 4;
            if (r < 0) r += 4;
            // Now r is 0 for same direction between start and end, 1 for one left turn in total, 2 for two turns (180 degrees), and 3 for one right turn.
            return DirectionToNucleotideArray[r];
        }
    }
}
