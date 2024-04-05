using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Typogenetics.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private Key[] AllowedKeys = new[]
        {
            Key.A,
            Key.C,
            Key.G,
            Key.T,
            Key.Delete,
            Key.Back,
            Key.Left,
            Key.Right,
            Key.Home,
            Key.End,
            Key.Enter,
            Key.Insert,
        };

        private void GattacaKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) return;
            if (!AllowedKeys.Contains(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}
