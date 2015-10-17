using System.Windows.Controls;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;
using System.Windows;

namespace LE.Visuals.Board
{
    /// <summary>
    /// Interaction logic for Fortress.xaml
    /// </summary>
    public partial class Fortress : UserControl, ITileControl
    {
        public TileType TileType
        {
            get
            {
                return TileType.none;
            }
        }

        public Fortress()
        {
            InitializeComponent();
        }

        public void ShowSelected(bool selected)
        {
            if (selected)
            {
                Unselected.Visibility = Visibility.Collapsed;
                Selected.Visibility = Visibility.Visible;
            }
            else
            {
                Unselected.Visibility = Visibility.Visible;
                Selected.Visibility = Visibility.Collapsed;
            }

        }
    }
}
