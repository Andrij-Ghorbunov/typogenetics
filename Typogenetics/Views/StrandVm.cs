using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typogenetics.Engine;

namespace Typogenetics.Views
{
    public class StrandVm : ViewModelBase
    {
        private string _data = string.Empty;

        public string Data
        {
            get => _data;
            set => SetValue(ref _data, value, DataChanged);
        }

        private static readonly char[] AllowedChars = new[] { 'A', 'C', 'T', 'G' };

        private void DataChanged(string oldValue, string newValue)
        {
            if (newValue.Any(it => !AllowedChars.Contains(it)))
            {
                var correctedValue = new string(newValue.Where(it => AllowedChars.Contains(it)).ToArray());
                Data = correctedValue;
            }
        }

        public Strand GetEntity()
        {
            var r = new Strand();
            r.AddRange(Data.Select(it => Enum.Parse<NucleotideEnum>($"{it}", true)));
            return r;
        }

        public static StrandVm Construct(Strand strand)
        {
            return new StrandVm { Data = string.Join(string.Empty, strand) };
        }
    }
}
