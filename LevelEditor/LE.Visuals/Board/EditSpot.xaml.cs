using System.Windows.Controls;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;

namespace LE.Visuals.Board
{
    /// <summary>
    /// Interaction logic for EditSpot.xaml
    /// </summary>
    public partial class EditSpot : UserControl, ITileControl
    {
        HexagonTile _currentTile;
        public HexagonTile CurrentTile {get;set;}

        public double X
        {
            get
            {
                return Canvas.GetLeft(this);
            }
        }

        public double Y
        {
            get
            {
                return Canvas.GetTop(this);
            }
        }

        public TileType TileType
        {
            get
            {
                return TileType.none;
            }
        }

        public EditSpot()
        {
            InitializeComponent();
        }


        public void SetLinkCount()
        {
            int count = 0;

            if (CurrentTile != null)
            {
                HexagonTile tile = CurrentTile;

                HexagonTile[] neighbours =
                    new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

                foreach (HexagonTile neighbour in neighbours)
                {
                    if (neighbour != null)
                    {
                        count++;
                    }
                }
            }

            Links.Text = count.ToString();
        }
    }
}
