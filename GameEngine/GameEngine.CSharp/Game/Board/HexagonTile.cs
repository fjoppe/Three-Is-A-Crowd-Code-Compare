using System;
using System.Runtime.Serialization;
namespace GameEngine.CSharp.Game.Board
{
    [DataContract(IsReference=true)]
    public class HexagonTile
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public HexagonTile NorthWest { get; set; }

        [DataMember]
        public HexagonTile North { get; set; }

        [DataMember]
        public HexagonTile NorthEast { get; set; }

        [DataMember]
        public HexagonTile SouthWest { get; set; }

        [DataMember]
        public HexagonTile South { get; set; }

        [DataMember]
        public HexagonTile SouthEast { get; set; }

        [DataMember]
        public TileType TileType { get; set; }

        [DataMember]
        public int X { get; set; }

        [DataMember]
        public int Y { get; set; }

        [DataMember]
        public int TileValue { get; set; }

        [DataMember]
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
