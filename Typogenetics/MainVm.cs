using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typogenetics.Views;

namespace Typogenetics
{
    public class MainVm : ViewModelBase
    {
        public GameVm Game { get; }

        public MainVm()
        {
            Game = new GameVm();
        }
    }
}
