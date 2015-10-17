using GameEngine.CSharp.Game.Board;
using System;
using System.Runtime.Serialization;

namespace GameEngine.CSharp.Game.Engine
{
    [DataContract]
    public class TileColor
    {
        [DataMember]
        public int id { get; set; }
        
        [DataMember]
        public TileType color { get; set; }

        [DataMember]
        public bool Fortress { get; set; }
    }
}
