using System.Windows.Controls;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;

namespace LE.Visuals.Board
{
    public partial class Blue : UserControl, ITileControl
    {
        public TileType TileType
        {
            get
            {
                return TileType.blue;
            }
        }

 
        public Blue()
        {
            InitializeComponent();
        }
    }
}
