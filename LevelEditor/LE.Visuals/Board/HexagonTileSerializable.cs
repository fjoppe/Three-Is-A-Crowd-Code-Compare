
using System;
namespace TIAC.Visuals.Board
{
    [Serializable]
    public class HexagonTileSerializable
    {
        //public bool Active { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public TileType TileType { get; set; }

        public HexagonTileSerializable(HexagonTile source)
        {
            //this.Active = source.Active;
            this.X = source.Visual.X;
            this.Y = source.Visual.Y;
            this.TileType = source.TileType;
        }
    }
}
