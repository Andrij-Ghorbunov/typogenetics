using Ronix.Framework.Mvvm;
using Typogenetics.Engine;

namespace Typogenetics.Views
{
    public class EnzymeVm : ViewModelBase
    {
        public readonly Enzyme Data;
        public string Name { get; }
        public EnzymeVm(Enzyme data)
        {
            Data = data;
            Name = Data.Name;
        }
    }
}
