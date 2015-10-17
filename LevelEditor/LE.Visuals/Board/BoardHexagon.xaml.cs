using System.Windows;
using System.Windows.Controls;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;

namespace LE.Visuals.Board
{
    public partial class BoardHexagon : UserControl
    {
        public HexagonTile CurrentTile{get;set;}


        public void SetTileType(TileType tileType)
        {
            EditSpot.Visibility = Visibility.Collapsed;
            Board.Visibility = Visibility.Collapsed;
            Red.Visibility = Visibility.Collapsed;
            Blue.Visibility = Visibility.Collapsed;
            Yellow.Visibility = Visibility.Collapsed;

            switch (tileType)
            {
                case TileType.none:
                    EditSpot.Visibility = Visibility.Visible;
                    break;
                case TileType.board:
                    Board.Visibility = Visibility.Visible;
                    break;
                case TileType.red:
                    Red.Visibility = Visibility.Visible;
                    break;
                case TileType.blue:
                    Blue.Visibility = Visibility.Visible;
                    break;
                case TileType.yellow:
                    Yellow.Visibility = Visibility.Visible;
                    break;
            }
        }


        public BoardHexagon()
        {
            InitializeComponent();
        }

        public void SetLinkCount(int count)
        {
            this.Links.Text = count.ToString();
        }

        public void SetTileValue(int value)
        {
            this.TileValue.Text = value.ToString();
            this.TileValueShadow.Text = value.ToString();
        }


        public void ShowLinks(bool show)
        {
            if (show)
            {
                this.Links.Visibility = Visibility.Visible;
            }
            else
            {
                this.Links.Visibility = Visibility.Collapsed;
            }
        }


        public void ShowTileValues(bool show)
        {
            if (show)
            {
                this.TileValue.Visibility = Visibility.Visible;
                this.TileValueShadow.Visibility = Visibility.Visible;
            }
            else
            {
                this.TileValue.Visibility = Visibility.Collapsed;
                this.TileValueShadow.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowFortress(bool show)
        {
            if (show)
            {
                this.Fortress.Visibility = Visibility.Visible;
            }
            else
            {
                this.Fortress.Visibility = Visibility.Collapsed;
            }
        }
    }
}
