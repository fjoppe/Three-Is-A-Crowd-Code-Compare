using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameEngine.CSharp.Game.Board
{
    [DataContract(Namespace = "urn:TIC")]
    public class BoardSerializable
    {
        [DataMember(Name="FortressesPerPlayer")]
        public int FortressesPerPlayer { get; set; }

        [DataMember(Name="ActiveTileList")]
        public List<HexagonTileSerializable> ActiveTileList=new List<HexagonTileSerializable>();
    }
}
