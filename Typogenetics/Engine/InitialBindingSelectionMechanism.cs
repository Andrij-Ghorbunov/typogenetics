using System;
using System.Collections.Generic;
using System.Linq;

namespace Typogenetics.Engine
{
    /// <summary>
    /// Hofstadter doesn't say anything about how to select the specific place to initially bind the enzyme to a strand
    /// if several places are acceptable. So here we'll have a static switch for several such mechanisms.
    /// </summary>
    public static class InitialBindingSelectionMechanism
    {
        public enum InitialBindingSelectionModeEnum
        {
            AlwaysFirst,
            AlwaysLast,
            AlwaysMiddle,
            Random,
            NthOrLast
        }

        private static readonly Random Random = new();

        public static InitialBindingSelectionModeEnum CurrentMode { get; set; } = InitialBindingSelectionModeEnum.Random;

        public static int N { get; set; }

        public static Unit Select(List<Unit> candidates)
        {
            switch (CurrentMode)
            {
                case InitialBindingSelectionModeEnum.AlwaysFirst: return candidates.First();
                case InitialBindingSelectionModeEnum.AlwaysMiddle: return candidates[candidates.Count / 2];
                case InitialBindingSelectionModeEnum.AlwaysLast: return candidates.Last();
                case InitialBindingSelectionModeEnum.Random: return candidates[Random.Next(candidates.Count)];
                case InitialBindingSelectionModeEnum.NthOrLast: return candidates[Math.Min(N, candidates.Count - 1)];
            }
            throw new Exception("Unknown selection mode");
        }
    }
}
