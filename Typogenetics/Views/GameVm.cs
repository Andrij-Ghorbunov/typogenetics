using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Typogenetics.Engine;
using static Typogenetics.Engine.InitialBindingSelectionMechanism;

namespace Typogenetics.Views
{
    public class GameVm : ViewModelBase
    {
        public ObservableCollection<StrandVm> Strands { get; }
        public ObservableCollection<EnzymeVm> Enzymes { get; }

        private readonly HashSet<string> _strands = new HashSet<string>();

        private StrandVm? _selectedStrand;

        public StrandVm? SelectedStrand
        {
            get => _selectedStrand;
            set => SetValue(ref _selectedStrand, value, SelectedStrandChanged);
        }

        private void SelectedStrandChanged()
        {
            RibosomeSingle.RaiseCanExecuteChanged();
            Process.RaiseCanExecuteChanged();
        }

        private EnzymeVm? _selectedEnzyme;

        public EnzymeVm? SelectedEnzyme
        {
            get => _selectedEnzyme;
            set => SetValue(ref _selectedEnzyme, value, SelectedEnzymeChanged);
        }

        private void SelectedEnzymeChanged()
        {
            Process.RaiseCanExecuteChanged();
        }

        private bool _autoRemoveDuplicateStrands;

        public bool AutoRemoveDuplicateStrands
        {
            get => _autoRemoveDuplicateStrands;
            set => SetValue(ref _autoRemoveDuplicateStrands, value);
        }

        private bool _autoRemoveShortStrands;

        public bool AutoRemoveShortStrands
        {
            get => _autoRemoveShortStrands;
            set => SetValue(ref _autoRemoveShortStrands, value);
        }

        public ObservableCollection<InitialBindingSelectionModeEnum> InitialBindingSelectionModes { get; }

        private InitialBindingSelectionModeEnum _initialBindingSelectionMode = CurrentMode;

        public InitialBindingSelectionModeEnum InitialBindingSelectionMode
        {
            get => _initialBindingSelectionMode;
            set => SetValue(ref _initialBindingSelectionMode, value, InitialBindingSelectionModeChanged);
        }

        private void InitialBindingSelectionModeChanged(InitialBindingSelectionModeEnum newValue)
        {
            CurrentMode = newValue;
            IsInitialBindingNEditable = newValue == InitialBindingSelectionModeEnum.NthOrLast;
        }

        private bool _isInitialBindingNEditable = CurrentMode == InitialBindingSelectionModeEnum.NthOrLast;

        public bool IsInitialBindingNEditable
        {
            get => _isInitialBindingNEditable;
            set => SetValue(ref _isInitialBindingNEditable, value);
        }

        private int _initialBindingN = N + 1;

        public int InitialBindingN
        {
            get => _initialBindingN;
            set => SetValue(ref _initialBindingN, value, InitialBindingNChanged);
        }

        private void InitialBindingNChanged(int newValue)
        {
            N = newValue - 1;
        }

        public IDelegateCommand RemoveStrand { get; }
        public IDelegateCommand RemoveEnzyme { get; }
        public IDelegateCommand RibosomeSingle { get; }
        public IDelegateCommand RibosomeAll { get; }
        public IDelegateCommand Process { get; }
        public IDelegateCommand ClearStrands { get; }
        public IDelegateCommand ClearEnzymes { get; }
        public IDelegateCommand RemoveDuplicateStrands { get; }
        public IDelegateCommand RemoveShortStrands { get; }

        public GameVm()
        {
            Strands = new ObservableCollection<StrandVm>();
            Enzymes = new ObservableCollection<EnzymeVm>();

            InitialBindingSelectionModes = new ObservableCollection<InitialBindingSelectionModeEnum>(Enum.GetValues<InitialBindingSelectionModeEnum>());

            RemoveStrand = new Command<StrandVm>(RemoveStrandCommand);
            RemoveEnzyme = new Command<EnzymeVm>(RemoveEnzymeCommand);
            RibosomeSingle = new Command(RibosomeSingleCommand, RibosomeSinglePossible);
            RibosomeAll = new Command(RibosomeAllCommand);
            Process = new Command(ProcessCommand, ProcessPossible);
            ClearStrands = new Command(ClearStrandsCommand);
            ClearEnzymes = new Command(ClearEnzymesCommand);
            RemoveDuplicateStrands = new Command(RemoveDuplicateStrandsCommand);
            RemoveShortStrands = new Command(RemoveShortStrandsCommand);
        }

        private void RemoveStrandCommand(StrandVm strand)
        {
            Strands.Remove(strand);
        }

        private void RemoveEnzymeCommand(EnzymeVm enzyme)
        {
            Enzymes.Remove(enzyme);
        }

        private void RibosomeSingleCommand()
        {
            if (SelectedStrand == null) return;
            var enzymes = Ribosome.Construct(SelectedStrand.GetEntity());
            var vms = enzymes.Select(it => new EnzymeVm(it)).ToList();
            SelectedEnzyme = null;
            foreach (var vm in vms)
            {
                Enzymes.Add(vm);
                if (SelectedEnzyme == null) SelectedEnzyme = vm;
            }
            if (!enzymes.Any())
            {
                MessageBox.Show("No enzymes coded in the selected strand!");
            }
        }

        private bool RibosomeSinglePossible()
        {
            return SelectedStrand != null;
        }

        private void RibosomeAllCommand()
        {
            SelectedEnzyme = null;
            Enzymes.Clear();
            var enzymes = new List<Enzyme>();
            foreach (var strand in Strands)
            {
                enzymes.AddRange(Ribosome.Construct(strand.GetEntity()));
            }
            foreach (var enzyme in enzymes)
            {
                Enzymes.Add(new EnzymeVm(enzyme));
            }
        }

        private void ProcessCommand()
        {
            if (SelectedEnzyme == null || SelectedStrand == null) return;
            HashSet<string>? hash = null;
            var enzyme = SelectedEnzyme.Data;
            var strand = SelectedStrand.GetEntity();
            var newStrands = enzyme.Process(strand);
            Strands.Remove(SelectedStrand);
            if (AutoRemoveDuplicateStrands)
                hash = new HashSet<string>(Strands.Select(it => it.Data));
            foreach (var newStrand in newStrands)
            {
                var vm = StrandVm.Construct(newStrand);
                if (AutoRemoveShortStrands && vm.Data.Length < 2) continue;
                if (hash != null)
                {
                    if (hash.Contains(vm.Data)) continue;
                    hash.Add(vm.Data);
                }
                Strands.Add(vm);
                if (SelectedStrand == null) SelectedStrand = vm;
            }
        }

        private bool ProcessPossible()
        {
            return SelectedEnzyme != null && SelectedStrand != null;
        }

        private void ClearStrandsCommand()
        {
            Strands.Clear();
        }

        private void ClearEnzymesCommand()
        {
            Enzymes.Clear();
        }

        private void RemoveDuplicateStrandsCommand()
        {
            var hash = new HashSet<string>(Strands.Select(it => it.Data));
            foreach (var strand in Strands.ToList())
            {
                if (hash.Contains(strand.Data))
                    hash.Remove(strand.Data);
                else
                    Strands.Remove(strand);
            }
        }

        private void RemoveShortStrandsCommand()
        {
            foreach (var strand in Strands.ToList())
            {
                if (strand.Data.Length < 2)
                    Strands.Remove(strand);
            }
        }
    }
}
