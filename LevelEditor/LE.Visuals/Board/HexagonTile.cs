using System.Windows;

namespace TIAC.Visuals.Board
{
    public class HexagonTile
    {
        public HexagonTile NorthWest { get; set; }
        public HexagonTile North { get; set; }
        public HexagonTile NorthEast { get; set; }

        public HexagonTile SouthWest { get; set; }
        public HexagonTile South { get; set; }
        public HexagonTile SouthEast { get; set; }

        public TileType TileType { get; set; }

        private ITileControl visual;
        public ITileControl Visual {
            get
            {
                return this.visual;
            }
            set
            {
                this.visual = value;
                this.visual.CurrentTile = this;
            }
        }

//        public bool Active { get; set; }
    }
}
