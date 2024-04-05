using System.Collections.Generic;
using System.Linq;

namespace Typogenetics.Engine
{
    public class Enzyme
    {
        public string Name { get; }
        public NucleotideEnum InitialBinding { get; }
        public bool IsCopyMode { get; private set; }
        public bool Finished { get; private set; }

        private Unit BoundBase { get; set; }
        private readonly AminoAcidEnum[] _commands;

        /// <summary>
        /// We need to keep track of all actual units, so 1) we can restore all constructed strands later,
        /// 2) lysosomes, err, Garbage Collector doesn't eat them
        /// </summary>
        public List<Unit> OperatedUnits { get; } = new List<Unit>();

        public Enzyme(NucleotideEnum initialBinding, AminoAcidEnum[] commands)
        {
            _commands = commands;
            InitialBinding = initialBinding;
            Name = InitialBinding.ToString() + ":" + string.Join("-", commands).ToLowerInvariant();
        }

        public List<Strand> Process(Strand strand)
        {
            Clean();
            LoadStrand(strand);
            FindStart();
            ExecuteAllCommands();
            return ConstructStrands();
        }

        private void Clean()
        {
            OperatedUnits.Clear();
            IsCopyMode = false;
            Finished = false;
        }

        private void LoadStrand(Strand strand)
        {
            Unit? unit = null;
            foreach (var nucleotide in strand)
            {
                var newUnit = new Unit();
                if (unit != null) unit.Right = newUnit;
                newUnit.Left = unit;
                newUnit.Base = nucleotide;
                unit = newUnit;
                OperatedUnits.Add(newUnit);
            }
        }

        private void FindStart()
        {
            var acceptableUnits = OperatedUnits.Where(it => it.Base == InitialBinding).ToList();
            if (!acceptableUnits.Any())
            {
                Finished = true;
                return;
            }
            if (acceptableUnits.Count == 1)
            {
                BoundBase = acceptableUnits.Single();
            }
            else
            {
                BoundBase = InitialBindingSelectionMechanism.Select(acceptableUnits);
            }
        }

        private void ExecuteAllCommands()
        {
            foreach (var aminoAcid in _commands)
            {
                if (Finished) return;
                ExecuteCommand(aminoAcid);
            }
        }

        private List<Strand> ConstructStrands()
        {
            var r = new List<Strand>();
            while (OperatedUnits.Any())
            {
                // take any unit from the operated ones
                var aUnit = OperatedUnits.First();

                // go as far to the left from it in the chain as we can
                while (aUnit.Left != null) aUnit = aUnit.Left;

                // start constructing the strand
                var strand = new Strand();

                // move from left to right and collect all the bases
                while (aUnit != null)
                {
                    // save the base from the unit
                    strand.Add(aUnit.Base);
                    // remove the unit from operated, so we don't count it again
                    OperatedUnits.Remove(aUnit);
                    // move to the right
                    aUnit = aUnit.Right;
                }

                // no complementary units touched, so this connection is not actual now

                // store the found strand
                r.Add(strand);
            }
            return r;
        }

        private void ExecuteCommand(AminoAcidEnum command)
        {
            switch (command)
            {
                case AminoAcidEnum.Cut: Cut(); return;
                case AminoAcidEnum.Del: Delete(); return;
                case AminoAcidEnum.Swi: Switch(); return;
                case AminoAcidEnum.Mvr: MoveRight(); return;
                case AminoAcidEnum.Mvl: MoveLeft(); return;
                case AminoAcidEnum.Cop: TurnCopyOn(); return;
                case AminoAcidEnum.Off: TurnCopyOff(); return;
                case AminoAcidEnum.Ina: Insert(NucleotideEnum.A); return;
                case AminoAcidEnum.Inc: Insert(NucleotideEnum.C); return;
                case AminoAcidEnum.Ing: Insert(NucleotideEnum.G); return;
                case AminoAcidEnum.Int: Insert(NucleotideEnum.T); return;
                case AminoAcidEnum.Rpy: SearchPyrimidineRight(); return;
                case AminoAcidEnum.Rpu: SearchPurineRight(); return;
                case AminoAcidEnum.Lpy: SearchPyrimidineLeft(); return;
                case AminoAcidEnum.Lpu: SearchPurineLeft(); return;
            }
        }

        private void MoveRight()
        {
            var right = BoundBase.Right;
            if (right == null)
            {
                Finished = true;
                return;
            }
            BoundBase = right;
            if (IsCopyMode)
            {
                Copy();
            }
        }

        private void MoveLeft()
        {
            var left = BoundBase.Left;
            if (left == null)
            {
                Finished = true;
                return;
            }
            BoundBase = left;
            if (IsCopyMode)
            {
                Copy();
            }
        }

        private void Insert(NucleotideEnum n)
        {
            if (BoundBase.Right == null)
            {
                var r = new Unit();
                BoundBase.Right = r;
                r.Left = BoundBase;
                r.Complementary = BoundBase.Complementary?.Left;
                OperatedUnits.Add(r);
            }
            BoundBase.Right.Base = n;
            MoveRight();
        }

        private void Delete()
        {
            var right = BoundBase.Right;
            if (BoundBase.Left != null)
            {
                BoundBase.Left.Right = null;
            }
            if (BoundBase.Right != null)
            {
                BoundBase.Right.Left = null;
            }
            if (BoundBase.Complementary != null)
            {
                BoundBase.Complementary.Complementary = null;
            }
            OperatedUnits.Remove(BoundBase);
            if (right == null)
            {
                Finished = true;
            }
            else
            {
                BoundBase = right;
                if (IsCopyMode)
                {
                    Copy();
                }
            }
        }

        private void Cut()
        {
            if (BoundBase.Right != null)
            {
                BoundBase.Right.Left = null;
                BoundBase.Right = null;
            }
            if (BoundBase.Complementary != null && BoundBase.Complementary.Left != null)
            {
                BoundBase.Complementary.Left.Right = null;
                BoundBase.Complementary.Left = null;
            }
        }

        private void Switch()
        {
            if (BoundBase.Complementary == null)
            {
                Finished = true;
                return;
            }
            BoundBase = BoundBase.Complementary;
        }

        private void SearchPyrimidineRight()
        {
            do
            {
                MoveRight();
            }
            while (!Finished && !Nucleotide.IsPyrimidine(BoundBase.Base));
        }

        private void SearchPyrimidineLeft()
        {
            do
            {
                MoveLeft();
            }
            while (!Finished && !Nucleotide.IsPyrimidine(BoundBase.Base));
        }


        private void SearchPurineRight()
        {
            do
            {
                MoveRight();
            }
            while (!Finished && !Nucleotide.IsPurine(BoundBase.Base));
        }

        private void SearchPurineLeft()
        {
            do
            {
                MoveLeft();
            }
            while (!Finished && !Nucleotide.IsPurine(BoundBase.Base));
        }

        private void TurnCopyOn()
        {
            IsCopyMode = true;
            Copy();
        }

        private void TurnCopyOff()
        {
            IsCopyMode = false;
        }

        private void Copy()
        {
            if (BoundBase.Complementary == null)
            {
                var c = new Unit();
                BoundBase.Complementary = c;
                c.Complementary = BoundBase;
                if (BoundBase.Left?.Complementary != null)
                {
                    c.Right = BoundBase.Left.Complementary;
                    BoundBase.Left.Complementary.Left = c;
                }
                if (BoundBase.Right?.Complementary != null)
                {
                    c.Left = BoundBase.Right.Complementary;
                    BoundBase.Right.Complementary.Right = c;
                }
                OperatedUnits.Add(c);
            }
            BoundBase.Complementary.Base = Nucleotide.Complementary(BoundBase.Base);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
