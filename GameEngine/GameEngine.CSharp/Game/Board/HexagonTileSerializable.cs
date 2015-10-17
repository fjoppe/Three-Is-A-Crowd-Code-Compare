using System;
using System.Runtime.Serialization;

namespace GameEngine.CSharp.Game.Board
{
    [DataContract(Namespace = "urn:TIC")]
    public class HexagonTileSerializable
    {
        public HexagonTileSerializable()
        {
        }

        [DataMember(Name="Id")]
        public int Id { get; set; }

        [DataMember(Name = "X")]
        public int X { get; set; }

        [DataMember(Name = "Y")]
        public int Y { get; set; }

        [DataMember(Name = "TileType")]
        public TileType TileType { get; set; }

        [DataMember(Name = "TileValue")]
        public int TileValue { get; set; }

        [DataMember(Name = "Fortress")]
        public bool Fortress { get; set; }

        public HexagonTileSerializable(HexagonTile source)
        {
            this.Id = source.Id;
            this.X = source.X;
            this.Y = source.Y;
            this.TileType = source.TileType;
            this.TileValue = source.TileValue;
            this.Fortress = source.Fortress;
        }
    }
}
