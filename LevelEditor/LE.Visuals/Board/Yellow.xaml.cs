using System.Windows.Controls;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;

namespace LE.Visuals.Board
{
    public partial class Yellow : UserControl, ITileControl
    {
        public TileType TileType
        {
            get
            {
                return TileType.yellow;
            }
        }

        public Yellow()
        {
            InitializeComponent();
        }
    }
}
