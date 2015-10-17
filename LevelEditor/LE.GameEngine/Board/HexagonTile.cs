using LE.GameEngine.TIC_Webservice;
namespace LE.GameEngine.board
{
    public class HexagonTile
    {
        public int Id { get; set; }
        public HexagonTile NorthWest { get; set; }
        public HexagonTile North { get; set; }
        public HexagonTile NorthEast { get; set; }

        public HexagonTile SouthWest { get; set; }
        public HexagonTile South { get; set; }
        public HexagonTile SouthEast { get; set; }

        public TileType TileType { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int TileValue { get; set; }

        public bool Fortress { get; set; }

        public int LinkCount
        {
            get
            {
                int count = 0;

                HexagonTile[] neighbours =
                    new[]{
                    North,
                    NorthEast,
                    NorthWest,
                    South,
                    SouthEast,
                    SouthWest,
                };

                foreach (HexagonTile neighbour in neighbours)
                {
                    if (neighbour != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}
